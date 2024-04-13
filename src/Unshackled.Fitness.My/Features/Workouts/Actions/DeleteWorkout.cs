using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class DeleteWorkout
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
			long workoutId = request.Sid.DecodeLong();

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (workout == null)
				return new CommandResult(false, "Invalid workout.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				await db.WorkoutSets
					.Where(x => x.WorkoutId == workout.Id)
					.DeleteFromQueryAsync(cancellationToken);

				await db.WorkoutSetGroups
					.Where(x => x.WorkoutId == workout.Id)
					.DeleteFromQueryAsync(cancellationToken);

				await db.WorkoutTasks
					.Where(x => x.WorkoutId == workout.Id)
					.DeleteFromQueryAsync(cancellationToken);

				db.Workouts.Remove(workout);
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Workout deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "Database error. Workout could not be deleted.");
			}
		}
	}
}