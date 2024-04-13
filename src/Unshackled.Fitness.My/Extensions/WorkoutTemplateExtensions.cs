using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Extensions;

public static class WorkoutTemplateExtensions
{
	public static async Task<CommandResult<string>> AddWorkoutFromTemplate(this BaseDbContext db,
		long memberId, long templateId, CancellationToken cancellationToken)
	{
		var template = await db.WorkoutTemplates
				.Include(x => x.Groups)
				.Include(x => x.Sets)
					.ThenInclude(x => x.Exercise)
				.Include(x => x.Tasks)
				.Where(x => x.Id == templateId && x.MemberId == memberId)
				.SingleOrDefaultAsync();

		if (template == null)
			return new CommandResult<string>(false, "Template not found.");

		WorkoutEntity workout = new()
		{
			MemberId = memberId,
			Title = template.Title,
			MusclesTargeted = template.MusclesTargeted,
			ExerciseCount = template.ExerciseCount,
			SetCount = template.SetCount,
			WorkoutTemplateId = templateId
		};

		using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
		try
		{
			// Save so we get new workout ID
			db.Workouts.Add(workout);
			await db.SaveChangesAsync(cancellationToken);

			// Create map of template group ids to workout group ids
			Dictionary<long, long> groupIdMap = new();

			// Create new groups
			foreach (var group in template.Groups)
			{
				WorkoutSetGroupEntity g = new()
				{
					MemberId = memberId,
					SortOrder = group.SortOrder,
					Title = group.Title,
					WorkoutId = workout.Id
				};
				db.WorkoutSetGroups.Add(g);
				await db.SaveChangesAsync(cancellationToken);

				// Add to id map
				groupIdMap.Add(group.Id, g.Id);
			}

			// Create new sets
			foreach (var set in template.Sets)
			{
				// Add set with new workout ID
				WorkoutSetEntity s = new()
				{
					ExerciseId = set.ExerciseId,
					SetMetricType = set.SetMetricType,
					ListGroupId = groupIdMap[set.ListGroupId],
					IntensityTarget = set.IntensityTarget,
					IsTrackingSplit = set.Exercise.IsTrackingSplit,
					MemberId = memberId,
					RepMode = set.RepMode,
					RepsTarget = set.RepsTarget,
					SecondsTarget = set.SecondsTarget,
					SetType = set.SetType,
					SortOrder = set.SortOrder,
					WorkoutId = workout.Id
				};

				// Save so we get new set ID
				db.WorkoutSets.Add(s);
				await db.SaveChangesAsync();
			}

			// Add tasks
			if (template.Tasks.Any())
			{
				db.WorkoutTasks.AddRange(template.Tasks
					.Select(x => new WorkoutTaskEntity
					{
						MemberId = memberId,
						SortOrder = x.SortOrder,
						Text = x.Text,
						Type = x.Type,
						WorkoutId = workout.Id
					})
					.ToList());

				await db.SaveChangesAsync();
			}

			await transaction.CommitAsync();
			return new CommandResult<string>(true, "Workout created.", workout.Id.Encode());
		}
		catch
		{
			return new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
