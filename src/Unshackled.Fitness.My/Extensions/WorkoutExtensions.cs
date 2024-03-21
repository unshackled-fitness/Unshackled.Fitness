using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;

namespace Unshackled.Fitness.My.Extensions;

public static class WorkoutExtensions
{
	public static async Task UpdateWorkoutRecords(this BaseDbContext db, WorkoutEntity workout)
	{
		// Get sets
		var sets = await db.WorkoutSets
			.Where(x => x.WorkoutId == workout.Id && x.SetType == WorkoutSetTypes.Standard)
			.ToListAsync();

		workout.RecordSecondsAtWeightCount = 0;
		workout.RecordSecondsCount = 0;
		workout.RecordTargetVolumeCount = 0;
		workout.RecordTargetWeightCount = 0;
		workout.RecordVolumeCount = 0;
		workout.RecordWeightCount = 0;

		await GetBestSetsByWeight(db, workout, sets);
		await GetBestSetsByVolume(db, workout, sets);
		await GetBestSetsBySeconds(db, workout, sets);

		await db.SaveChangesAsync();
	}

	public static async Task UpdateWorkoutRecords(this BaseDbContext db, long workoutId)
	{
		var workout = await db.Workouts
			.Where(x => x.Id == workoutId)
			.SingleOrDefaultAsync();

		if (workout == null) return;

		await db.UpdateWorkoutRecords(workout);
	}

	public static async Task UpdateWorkoutStats(this BaseDbContext db, WorkoutEntity entity)
	{
		// Get set exercise data
		var sets = await db.WorkoutSets
			.AsNoTracking()
			.Include(x => x.Exercise)
			.Where(x => x.WorkoutId == entity.Id)
			.Select(x => new
			{
				x.Id,
				x.ExerciseId,
				x.Exercise.Muscles,
				x.Reps,
				x.RepsLeft,
				x.RepsRight,
				x.VolumeKg,
				x.VolumeLb
			})
			.ToListAsync();

		// Exercise count
		entity.ExerciseCount = sets.Select(x => x.ExerciseId).Distinct().Count();
		// Set count
		entity.SetCount = sets.Count;
		// Rep Count
		entity.RepCount = sets.Select(x => x.Reps + x.RepsLeft + x.RepsRight).Sum();

		List<MuscleTypes> muscles = new();
		foreach (var set in sets)
		{
			muscles.AddRange(EnumUtils.FromJoinedIntString<MuscleTypes>(set.Muscles));
		}

		// Distinct muscles targeted
		string[] musclesTargeted = muscles
			.OrderBy(x => x)
			.Select(x => x.Title())
			.Distinct().ToArray();

		if (musclesTargeted.Length > 0)
		{
			entity.MusclesTargeted = string.Join(", ", musclesTargeted);
		}
		else
		{
			entity.MusclesTargeted = null;
		}

		// Volume
		entity.VolumeLb = sets.Select(x => x.VolumeLb).Sum();
		entity.VolumeKg = sets.Select(x => x.VolumeKg).Sum();

		await db.SaveChangesAsync();
	}

	public static async Task UpdateWorkoutStats(this BaseDbContext db, long workoutId, long memberId)
	{
		var workout = await db.Workouts
			.Where(x => x.Id == workoutId && x.MemberId == memberId)
			.SingleOrDefaultAsync();

		if (workout == null) return;

		await db.UpdateWorkoutStats(workout);
	}

	private static async Task GetBestSetsBySeconds(this BaseDbContext db, WorkoutEntity workout, List<WorkoutSetEntity> sets)
	{
		var listBest = sets
			.OrderBy(x => x.ExerciseId)
			.ThenBy(x => x.DateRecordedUtc)
			.ToList();

		foreach (var set in listBest)
		{
			int maxSeconds = Math.Max(set.Seconds, Math.Max(set.SecondsLeft, set.SecondsRight));
			set.IsBestSetBySeconds = !sets
				.Where(x => x.Id != set.Id && x.ExerciseId == set.ExerciseId && (x.Seconds > maxSeconds || x.SecondsLeft > maxSeconds || x.SecondsRight > maxSeconds))
				.Any();

			if (set.IsBestSetBySeconds)
			{
				if (maxSeconds > 0)
				{
					set.IsRecordSeconds = !await db.WorkoutSets
						.Include(x => x.Workout)
						.Where(x => x.ExerciseId == set.ExerciseId
							&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
							&& x.WorkoutId != workout.Id
							&& (x.Seconds >= maxSeconds || x.SecondsLeft >= maxSeconds || x.SecondsRight >= maxSeconds))
						.AnyAsync();

					if (set.IsRecordSeconds)
						workout.RecordSecondsCount++;
				}
				else
				{
					set.IsRecordSeconds = false;
				}
			}
			else
			{
				set.IsRecordSeconds = false;
			}
		}

		long currentExerciseId = 0;
		listBest = sets
			.OrderBy(x => x.ExerciseId)
			.ThenByDescending(x => x.WeightKg)
			.ThenBy(x => x.DateRecordedUtc)
			.ToList();

		foreach (var set in listBest)
		{
			int maxSeconds = Math.Max(set.Seconds, Math.Max(set.SecondsLeft, set.SecondsRight));
			if (set.ExerciseId != currentExerciseId && maxSeconds > 0 && set.WeightKg > 0)
			{
				set.IsRecordSecondsAtWeight = !await db.WorkoutSets
					.Include(x => x.Workout)
					.Where(x => x.ExerciseId == set.ExerciseId
						&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
						&& x.WorkoutId != workout.Id
						&& x.WeightKg == set.WeightKg
						&& x.SetType == WorkoutSetTypes.Standard
						&& (x.Seconds >= maxSeconds || x.SecondsLeft >= maxSeconds || x.SecondsRight >= maxSeconds))
					.AnyAsync();

				if (set.IsRecordSecondsAtWeight)
					workout.RecordSecondsAtWeightCount++;
			}
			else
			{
				set.IsRecordSecondsAtWeight = false;
			}
			currentExerciseId = set.ExerciseId;
		}
	}

	private static async Task GetBestSetsByWeight(this BaseDbContext db, WorkoutEntity workout, List<WorkoutSetEntity> sets)
	{
		long currentExerciseId = 0;
		var listBest = sets
			.OrderBy(x => x.ExerciseId)
			.ThenByDescending(x => x.WeightKg)
			.ThenBy(x => x.DateRecordedUtc)
			.ToList();

		foreach (var set in listBest)
		{
			if (set.ExerciseId != currentExerciseId)
			{
				if (set.WeightKg > 0 || set.WeightLb > 0)
				{
					set.IsBestSetByWeight = true;

					if (set.RepsTarget > 0)
					{
						set.IsRecordTargetWeight = !await db.WorkoutSets
							.Include(x => x.Workout)
							.Where(x => x.ExerciseId == set.ExerciseId
								&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
								&& x.WorkoutId != workout.Id
								&& x.RepsTarget == set.RepsTarget
								&& x.SetType == WorkoutSetTypes.Standard
								&& x.WeightKg >= set.WeightKg).AnyAsync();

						if (set.IsRecordTargetWeight)
							workout.RecordTargetWeightCount++;
					}

					if (set.WeightKg > 0 || set.WeightLb > 0)
					{
						set.IsRecordWeight = !await db.WorkoutSets
							.Include(x => x.Workout)
							.Where(x => x.ExerciseId == set.ExerciseId
								&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
								&& x.WorkoutId != workout.Id
								&& x.WeightKg >= set.WeightKg).AnyAsync();
					}
					else
					{
						set.IsRecordWeight = false;
					}

					if (set.IsRecordWeight)
						workout.RecordWeightCount++;
				}
				else
				{
					set.IsBestSetByWeight = false;
					set.IsRecordTargetWeight = false;
					set.IsRecordWeight = false;
				}

				currentExerciseId = set.ExerciseId;
			}
			else
			{
				set.IsBestSetByWeight = false;
				set.IsRecordTargetWeight = false;
				set.IsRecordWeight = false;
			}
		}
	}

	private static async Task GetBestSetsByVolume(this BaseDbContext db, WorkoutEntity workout, List<WorkoutSetEntity> sets)
	{
		long currentExerciseId = 0;
		var listBest = sets
			.OrderBy(x => x.ExerciseId)
			.ThenByDescending(x => x.VolumeKg)
			.ThenBy(x => x.DateRecordedUtc)
			.ToList();

		foreach (var set in listBest)
		{
			if (set.ExerciseId != currentExerciseId)
			{
				if (set.WeightKg > 0 || set.WeightLb > 0)
				{
					set.IsBestSetByVolume = true;

					if (set.RepsTarget > 0)
					{
						set.IsRecordTargetVolume = !await db.WorkoutSets
							.Include(x => x.Workout)
							.Where(x => x.ExerciseId == set.ExerciseId
								&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
								&& x.WorkoutId != workout.Id
								&& x.RepsTarget == set.RepsTarget
								&& x.SetType == WorkoutSetTypes.Standard
								&& x.VolumeKg >= set.VolumeKg).AnyAsync();

						if (set.IsRecordTargetVolume)
							workout.RecordTargetVolumeCount++;
					}

					set.IsRecordVolume = !await db.WorkoutSets
						.Include(x => x.Workout)
						.Where(x => x.ExerciseId == set.ExerciseId
							&& x.Workout.DateCompletedUtc < workout.DateCompletedUtc
							&& x.WorkoutId != workout.Id
							&& x.VolumeKg >= set.VolumeKg).AnyAsync();

					if (set.IsRecordVolume)
						workout.RecordVolumeCount++;
				}
				else
				{
					set.IsBestSetByVolume = false;
					set.IsRecordTargetVolume = false;
					set.IsRecordVolume = false;
				}

				currentExerciseId = set.ExerciseId;
			}
			else
			{
				set.IsBestSetByVolume = false;
				set.IsRecordTargetVolume = false;
				set.IsRecordVolume = false;
			}
		}
	}
}
