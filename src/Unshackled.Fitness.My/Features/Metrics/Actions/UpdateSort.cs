using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class UpdateSort
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public UpdateSortModel Model { get; private set; }

		public Command(long memberId, UpdateSortModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var groups = await db.MetricDefinitionGroups
				.Where(x => x.MemberId == request.MemberId)
				.ToListAsync();

			var metrics = await db.MetricDefinitions
				.Where(x => x.MemberId == request.MemberId)
				.ToListAsync();

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Create map of string ids to long ids
				Dictionary<string, long> groupIdMap = groups
					.Select(x => x.Id)
					.ToDictionary(x => x.Encode());

				foreach (var group in request.Model.Groups)
				{
					var g = groups.Where(x => x.Id == group.Sid.DecodeLong())
						.SingleOrDefault();

					if (g == null && group.IsNew)
					{
						MetricDefinitionGroupEntity gEntity = new()
						{
							SortOrder = group.SortOrder,
							Title = group.Title,
							MemberId = request.MemberId
						};
						db.MetricDefinitionGroups.Add(gEntity);
						await db.SaveChangesAsync();
						groupIdMap.Add(group.Sid, gEntity.Id);
					}
					else if (g != null)
					{
						g.Title = group.Title;
						g.SortOrder = group.SortOrder;
						await db.SaveChangesAsync();
					}
				}

				foreach (var metric in request.Model.Metrics)
				{
					var m = metrics.Where(x => x.Id == metric.Sid.DecodeLong())
						.SingleOrDefault();

					if (m == null) continue;

					m.ListGroupId = groupIdMap[metric.ListGroupSid];
					m.SortOrder = metric.SortOrder;
					await db.SaveChangesAsync();
				}

				foreach (var group in request.Model.DeletedGroups)
				{
					var g = groups.Where(x => x.Id == group.Sid.DecodeLong())
						.SingleOrDefault();

					if (g == null) continue;

					// Check any metric definitions that might still be in group (should be 0 at this point)
					bool stopDelete = await db.MetricDefinitions
						.Where(x => x.ListGroupId == g.Id)
						.AnyAsync();

					if (!stopDelete)
					{
						db.MetricDefinitionGroups.Remove(g);
						await db.SaveChangesAsync();
					}
				}

				await transaction.CommitAsync(cancellationToken);
				return new CommandResult(true, "Metrics sort updated.");
			} 
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}