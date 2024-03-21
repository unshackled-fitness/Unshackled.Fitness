using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class SaveDefinition
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public FormMetricDefinitionModel Model { get; private set; }

		public Command(long memberId, FormMetricDefinitionModel model)
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
			long defId = request.Model.Sid.DecodeLong();
			long groupId = request.Model.ListGroupSid.DecodeLong();

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				bool groupExists = false;
				if (groupId > 0)
				{
					groupExists = await db.MetricDefinitionGroups
						.Where(x => x.Id == groupId && x.MemberId == request.MemberId)
						.AnyAsync();
				}
				else
				{
					// No group was passed, find member's last group
					var group = await db.MetricDefinitionGroups
						.Where(x => x.MemberId == request.MemberId)
						.OrderBy(x => x.SortOrder)
						.LastOrDefaultAsync();

					if (group == null)
					{
						// create default group
						group = new()
						{
							MemberId = request.MemberId,
							Title = "Metrics"
						};
						db.MetricDefinitionGroups.Add(group);
						await db.SaveChangesAsync();
					}
					groupId = group.Id;
					groupExists = true;
				}

				if (!groupExists)
					return new CommandResult(false, "Invalid group.");

				var definition = await db.MetricDefinitions
					.Where(x => x.Id == defId && x.MemberId == request.MemberId)
					.SingleOrDefaultAsync();

				string msg = "Metric ";
				if (definition == null)
				{
					int sortOrder = await db.MetricDefinitions
						.Where(x => x.ListGroupId == groupId)
						.OrderBy(x => x.SortOrder)
						.Select(x => x.SortOrder + 1)
						.LastOrDefaultAsync();

					// Update sort order of any definitions coming after new definition
					await db.MetricDefinitions
						.Where(x => x.MemberId == request.MemberId && x.SortOrder >= sortOrder)
						.UpdateFromQueryAsync(x => new MetricDefinitionEntity { SortOrder = x.SortOrder + 1 });

					definition = new MetricDefinitionEntity
					{
						ListGroupId = groupId,
						HighlightColor = request.Model.HighlightColor,
						IsArchived = false,
						MaxValue = request.Model.MaxValue,
						MemberId = request.MemberId,
						MetricType = request.Model.MetricType,
						SortOrder = sortOrder,
						SubTitle = request.Model.SubTitle?.Trim(),
						Title = request.Model.Title.Trim(),
					};
					db.MetricDefinitions.Add(definition);
					msg += "added.";
				}
				else
				{
					definition.HighlightColor = request.Model.HighlightColor;
					definition.MaxValue = request.Model.MaxValue;
					definition.MetricType = request.Model.MetricType;
					definition.SubTitle = request.Model.SubTitle?.Trim();
					definition.Title = request.Model.Title.Trim();
					msg += "updated.";
				}
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, msg);
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}