using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class UpdateSetSorts
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public UpdateSortsModel Model { get; private set; }

		public Command(long memberId, UpdateSortsModel model)
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
			long workoutId = request.Model.WorkoutSid.DecodeLong();

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult(false, "Invalid workout.");

			var groups = await db.WorkoutSetGroups
				.Where(x => x.WorkoutId == workoutId)
				.ToListAsync();

			var sets = await db.WorkoutSets
				.Where(x => x.WorkoutId == workoutId)
				.ToListAsync();

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Create map of string ids to long ids
				Dictionary<string, long> groupIdMap = groups
					.Select(x => x.Id)
					.ToDictionary(x => x.Encode());

				foreach (var item in request.Model.Groups)
				{
					var g = groups.Where(x => x.Id == item.Sid.DecodeLong())
						.SingleOrDefault();

					if (g == null && item.IsNew)
					{
						WorkoutSetGroupEntity gEntity = new()
						{
							MemberId = request.MemberId,
							SortOrder = item.SortOrder,
							Title = item.Title,
							WorkoutId = workoutId
						};
						db.WorkoutSetGroups.Add(gEntity);
						await db.SaveChangesAsync();
						groupIdMap.Add(item.Sid, gEntity.Id);
					}
					else if (g != null)
					{
						g.SortOrder = item.SortOrder;
						await db.SaveChangesAsync();
					}
				}

				foreach (var item in request.Model.Sets)
				{
					var s = sets.Where(x => x.Id == item.Sid.DecodeLong())
						.SingleOrDefault();

					if (s == null) continue;

					s.ListGroupId = groupIdMap[item.ListGroupSid];
					s.SortOrder = item.SortOrder;
					await db.SaveChangesAsync();
				}

				foreach (var item in request.Model.DeletedGroups)
				{
					var g = groups.Where(x => x.Id == item.Sid.DecodeLong())
						.SingleOrDefault();

					if (g == null) continue;

					// Check any sets that might still be in group (should be 0 at this point)
					bool stopDelete = await db.WorkoutSets
						.Where(x => x.ListGroupId == g.Id)
						.AnyAsync();

					if (!stopDelete)
					{
						db.WorkoutSetGroups.Remove(g);
						await db.SaveChangesAsync();
					}
				}

				await transaction.CommitAsync(cancellationToken);
				return new CommandResult(true, "Workout updated.");
			} 
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}