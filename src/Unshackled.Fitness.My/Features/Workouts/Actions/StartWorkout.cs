using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class StartWorkout
{
	public class Command : IRequest<CommandResult<DateTime>>
	{
		public long MemberId { get; private set; }
		public string WorkoutSid { get; private set; }

		public Command(long memberId, string workoutSid)
		{
			MemberId = memberId;
			WorkoutSid = workoutSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<DateTime>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<DateTime>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.WorkoutSid))
				return new CommandResult<DateTime>(false, "Invalid workout ID.");

			long workoutId = request.WorkoutSid.DecodeLong();

			if(workoutId == 0)
				return new CommandResult<DateTime>(false, "Invalid workout ID.");

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult<DateTime>(false, "Workout not found.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Mark pre-workout tasks as complete
				await db.WorkoutTasks
					.Where(x => x.WorkoutId == workoutId && x.Type == WorkoutTaskTypes.PreWorkout)
					.UpdateFromQueryAsync(x => new WorkoutTaskEntity { Completed = true });

				workout.DateStartedUtc = DateTime.UtcNow;
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult<DateTime>(true, "Workout started", workout.DateStartedUtc.Value);
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult<DateTime>(false, "Unexpected Error: Could not start the workout.");
			}
		}
	}
}