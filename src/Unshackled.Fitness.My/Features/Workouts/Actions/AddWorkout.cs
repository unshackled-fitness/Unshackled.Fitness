using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class AddWorkout
{
	public class Command : IRequest<CommandResult<string>>
	{
		public long MemberId { get; private set; }
		public string WorkoutSid { get; private set; }

		public Command(long memberId, string workoutSid)
		{
			MemberId = memberId;
			WorkoutSid = workoutSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			if (!string.IsNullOrEmpty(request.WorkoutSid)) // From previous workout
			{
				long workoutId = request.WorkoutSid.DecodeLong();

				if(workoutId == 0)
					return new CommandResult<string>(false, "Invalid workout ID.");

				var prevWorkout = await db.Workouts
					.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
					.SingleOrDefaultAsync();

				if (prevWorkout == null)
					return new CommandResult<string>(false, "Previous workout not found.");

				WorkoutEntity workout = new()
				{
					MemberId = request.MemberId,
					Title = prevWorkout.Title,
					MusclesTargeted = prevWorkout.MusclesTargeted,
					ExerciseCount = prevWorkout.ExerciseCount,
					SetCount = prevWorkout.SetCount,
					WorkoutTemplateId = prevWorkout.WorkoutTemplateId
				};

				try
				{
					// Save so we get new workout ID
					db.Workouts.Add(workout);
					await db.SaveChangesAsync(cancellationToken);

					// Get previous workout groups
					var prevGroups = await db.WorkoutSetGroups
						.AsNoTracking()
						.Where(x => x.WorkoutId == prevWorkout.Id)
						.OrderBy(x => x.SortOrder)
						.ToListAsync();

					// Create map of prev group ids to new group ids
					Dictionary<long, long> groupIdMap = new();

					// Create new groups
					foreach (var group in prevGroups)
					{
						// Add group with new workout ID
						WorkoutSetGroupEntity g = new()
						{
							MemberId = request.MemberId,
							SortOrder = group.SortOrder,
							Title = group.Title,
							WorkoutId = workout.Id
						};

						// Save so we get new set ID
						db.WorkoutSetGroups.Add(g);
						await db.SaveChangesAsync(cancellationToken);

						// Add to map
						groupIdMap.Add(group.Id, g.Id);
					}

					// Get previous workout sets
					var prevSets = await db.WorkoutSets
						.AsNoTracking()
						.Where(x => x.WorkoutId == prevWorkout.Id)
						.OrderBy(x => x.SortOrder)
						.ToListAsync();

					// Create new sets
					foreach (var set in prevSets)
					{
						// Add set with new workout ID
						WorkoutSetEntity s = new()
						{
							ExerciseId = set.ExerciseId,
							SetMetricType = set.SetMetricType,
							ListGroupId = groupIdMap[set.ListGroupId],
							MemberId = set.MemberId,
							IsTrackingSplit = set.IsTrackingSplit,
							RepMode = set.RepMode,
							RepsTarget = set.RepsTarget,
							SecondsTarget = set.SecondsTarget,
							SetType = set.SetType,
							SortOrder = set.SortOrder,
							WorkoutId = workout.Id
						};

						// Save so we get new set ID
						db.WorkoutSets.Add(s);
						await db.SaveChangesAsync(cancellationToken);
					}

					// Get previous workouts tasks
					var tasks = await db.WorkoutTasks
						.AsNoTracking()
						.Where(x => x.WorkoutId == prevWorkout.Id)
						.OrderBy(x => x.Type)
							.ThenBy(x => x.SortOrder)
						.Select(x => new WorkoutTaskEntity
						{
							MemberId = x.MemberId,
							SortOrder = x.SortOrder,
							Text = x.Text,
							Type = x.Type,
							WorkoutId = workout.Id
						})
						.ToListAsync();

					if (tasks.Any())
					{
						db.WorkoutTasks.AddRange(tasks);
						await db.SaveChangesAsync(cancellationToken);
					}

					await transaction.CommitAsync(cancellationToken);
					return new CommandResult<string>(true, "Workout created.", workout.Id.Encode());
				}
				catch
				{
					await transaction.RollbackAsync(cancellationToken);
					return new CommandResult<string>(false, Globals.UnexpectedError);
				}
			}
			else // From scratch
			{
				WorkoutEntity workout = new()
				{
					MemberId = request.MemberId,
					Title = "Workout"
				};

				try
				{
					db.Workouts.Add(workout);
					await db.SaveChangesAsync(cancellationToken);

					WorkoutSetGroupEntity group = new()
					{
						MemberId = request.MemberId,
						WorkoutId = workout.Id,
						SortOrder = 0,
						Title = string.Empty
					};
					db.WorkoutSetGroups.Add(group);
					await db.SaveChangesAsync(cancellationToken);

					await transaction.CommitAsync(cancellationToken);

					return new CommandResult<string>(true, "Workout created.", workout.Id.Encode());
				}
				catch
				{
					await transaction.RollbackAsync(cancellationToken);
					return new CommandResult<string>(false, Globals.UnexpectedError);
				}
			}
		}
	}
}