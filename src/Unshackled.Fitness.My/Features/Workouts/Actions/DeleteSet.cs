using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class DeleteSet
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string Sid { get; private set; }

		public Command(long memberId, string sid)
		{
			MemberId = memberId;
			Sid = sid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long setId = request.Sid.DecodeLong();

			var set = await db.WorkoutSets
				.Include(x => x.Workout)
				.Where(x => x.Id == setId && x.Workout.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (set == null)
				return new CommandResult(false, "Invalid set.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Adjust sort order for other sets in the workout with higher sort orders
				await db.WorkoutSets
					.Where(x => x.WorkoutId == set.WorkoutId && x.SortOrder > set.SortOrder)
					.UpdateFromQueryAsync(x => new WorkoutSetEntity { SortOrder = x.SortOrder - 1 });

				db.WorkoutSets.Remove(set);
				await db.SaveChangesAsync(cancellationToken);

				int remainingSetsInGroup = await db.WorkoutSets
					.Where(x => x.WorkoutId == set.WorkoutId && x.ListGroupId == set.ListGroupId)
					.CountAsync();

				int totalGroups = await db.WorkoutSetGroups
					.Where(x => x.WorkoutId == set.WorkoutId)
					.CountAsync();

				// Delete if no sets left in group and it is not the last remaining group in the workout
				if(remainingSetsInGroup == 0 && totalGroups > 1) 
				{
					await db.WorkoutSetGroups
						.Where(x => x.Id == set.ListGroupId)
						.DeleteFromQueryAsync();
				}

				await db.UpdateWorkoutStats(set.WorkoutId, request.MemberId);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Set deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}