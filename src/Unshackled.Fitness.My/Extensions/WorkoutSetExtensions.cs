using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Extensions;

public static class WorkoutSetExtensions
{
	public static FormWorkoutSetModel Map(this WorkoutSetEntity set, UnitSystems defaultUnits)
	{
		return new FormWorkoutSetModel
		{
			DateCreatedUtc = set.DateCreatedUtc,
			DateLastModifiedUtc = set.DateLastModifiedUtc,
			DateRecordedUtc = set.DateRecordedUtc,
			Equipment = set.Exercise != null ? EnumUtils.FromJoinedIntString<EquipmentTypes>(set.Exercise.Equipment) : new(),
			ExerciseNotes = set.Exercise?.Notes,
			SetMetricType = set.SetMetricType,
			ExerciseSid = set.ExerciseId.Encode(),
			ListGroupSid = set.ListGroupId.Encode(),
			IntensityTarget = set.IntensityTarget,
			IsExpanded = false,
			IsTrackingSplit = set.IsTrackingSplit,
			IsBestSetBySeconds = set.IsBestSetBySeconds,
			IsBestSetByVolume = set.IsBestSetByVolume,
			IsBestSetByWeight = set.IsBestSetByWeight,
			IsRecordSeconds = set.IsRecordSeconds,
			IsRecordSecondsAtWeight = set.IsRecordSecondsAtWeight,
			IsRecordTargetVolume = set.IsRecordTargetVolume,
			IsRecordTargetWeight = set.IsRecordTargetWeight,
			IsRecordVolume = set.IsRecordVolume,
			IsRecordWeight = set.IsRecordWeight,
			Muscles = set.Exercise != null ? EnumUtils.FromJoinedIntString<MuscleTypes>(set.Exercise.Muscles) : new(),
			RepMode = set.RepMode,
			Reps = set.Reps != 0 ? set.Reps : null,
			RepsLeft = set.RepsLeft != 0 ? set.RepsLeft : null,
			RepsTarget = set.RepsTarget,
			RepsRight = set.RepsRight != 0 ? set.RepsRight : null,
			Seconds = set.Seconds,
			SecondsLeft = set.SecondsLeft,
			SecondsRight = set.SecondsRight,
			SecondsTarget = set.SecondsTarget,
			SetType = set.SetType,
			SortOrder = set.SortOrder,
			Title = set.Exercise != null ? set.Exercise.Title : "Set",
			Sid = set.Id.Encode(),
			Weight = defaultUnits == UnitSystems.Metric
				? set.WeightKg != 0 ? set.WeightKg : null
				: set.WeightLb != 0 ? set.WeightLb : null,
			WeightUnit = defaultUnits == UnitSystems.Metric ? WeightUnits.kg : WeightUnits.lb,
			WorkoutSid = set.WorkoutId.Encode()
		};
	}

	public static List<FormWorkoutSetModel> Map(this List<WorkoutSetEntity> sets, UnitSystems defaultUnits)
	{
		return sets.Select(x => x.Map(defaultUnits)).ToList();
	}
}
