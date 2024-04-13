using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Extensions;

public static class ExerciseExtensions
{
	public static async Task<CommandResult> MergeExercises(this BaseDbContext db, long memberId, long idKept, long idDeleted)
	{
		var keptExercise = await db.Exercises
			.Where(x => x.Id == idKept && x.MemberId == memberId)
			.SingleOrDefaultAsync();

		var deletedExercise = await db.Exercises
			.Where(x => x.Id == idDeleted && x.MemberId == memberId)
			.SingleOrDefaultAsync();

		if (keptExercise != null && deletedExercise != null)
		{
			using (var transaction = db.Database.BeginTransaction())
			{
				try
				{
					if (!keptExercise.MatchId.HasValue && deletedExercise.MatchId.HasValue)
					{
						keptExercise.MatchId = deletedExercise.MatchId.Value;
					}

					// Update templates
					await db.WorkoutTemplateSets
						.Where(x => x.ExerciseId == deletedExercise.Id)
						.UpdateFromQueryAsync(x => new WorkoutTemplateSetEntity { ExerciseId = keptExercise.Id });

					// Update workouts
					await db.WorkoutSets
						.Where(x => x.ExerciseId == deletedExercise.Id)
						.UpdateFromQueryAsync(x => new WorkoutSetEntity { ExerciseId = keptExercise.Id });

					db.Exercises.Remove(deletedExercise);
					await db.SaveChangesAsync();

					// Commit transaction if all commands succeed, transaction will auto-rollback
					// when disposed if any command fails
					transaction.Commit();

					return new CommandResult(true, "Exercises successfully merged.");
				}
				catch
				{
					return new CommandResult(false, "An error occurred while processing the merge.");
				}
			}
		}

		return new CommandResult(false, "Could not complete merge.");
	}
}
