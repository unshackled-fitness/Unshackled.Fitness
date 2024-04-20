using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class CompleteWorkout
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public CompleteWorkoutModel Model { get; private set; }

		public Command(long memberId, CompleteWorkoutModel model)
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
			if (string.IsNullOrEmpty(request.Model.WorkoutSid))
				return new CommandResult(false, "Invalid workout ID.");

			long workoutId = request.Model.WorkoutSid.DecodeLong();

			if(workoutId == 0)
				return new CommandResult(false, "Invalid workout ID.");

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult(false, "Workout not found.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Mark post-workout tasks as complete
				await db.WorkoutTasks
					.Where(x => x.WorkoutId == workoutId && x.Type == WorkoutTaskTypes.PostWorkout)
					.UpdateFromQueryAsync(x => new WorkoutTaskEntity { Completed = true });

				workout.DateCompletedUtc = DateTime.UtcNow;
				workout.Rating = request.Model.Rating;
				workout.Notes = request.Model.Notes;
				await db.SaveChangesAsync(cancellationToken);

				// Calculate best sets and PR's
				await db.UpdateWorkoutRecords(workout);

				// Check if sequential programs exist where workout template ID is next template
				await SetNextWorkout(workout);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Workout finished.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "Unexpected Error: Could not complete the workout.");
			}
		}

		private async Task SetNextWorkout(WorkoutEntity workout)
		{
			if (workout.WorkoutTemplateId.HasValue)
			{
				var programTemplates = await db.ProgramTemplates
					.Include(x => x.Program)
					.Where(x => x.WorkoutTemplateId == workout.WorkoutTemplateId.Value
						&& x.Program.ProgramType == ProgramTypes.Sequential
						&& x.SortOrder == x.Program.NextTemplateIndex)
					.ToListAsync();

				foreach (var programTemplate in programTemplates)
				{
					// Get the next item in the list. Will return zero if not found.
					int nextIndex = await db.ProgramTemplates
						.Where(x => x.ProgramId == programTemplate.ProgramId
							&& x.SortOrder == programTemplate.SortOrder + 1)
						.Select(x => x.SortOrder)
						.SingleOrDefaultAsync();

					// Update program with new index and last workout date
					await db.Programs
						.Where(x => x.Id == programTemplate.ProgramId)
						.UpdateFromQueryAsync(x => new ProgramEntity
						{
							NextTemplateIndex = nextIndex,
							DateLastWorkoutUtc = workout.DateStartedUtc!.Value
						});
				}
			}
		}
	}
}