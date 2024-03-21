namespace Unshackled.Fitness.Core.Enums;

public enum SetMetricTypes
{
	WeightReps = 0,
	RepsOnly = 1,
	WeightTime = 2,
	TimeOnly = 3
}

public static class SetTypesExtensions
{
	public static string Title(this SetMetricTypes setType)
	{
		return setType switch
		{
			SetMetricTypes.WeightReps => "Weight/Reps",
			SetMetricTypes.RepsOnly => "Reps Only",
			SetMetricTypes.WeightTime => "Weight/Time",
			SetMetricTypes.TimeOnly => "Time Only",
			_ => string.Empty
		};
	}

	public static bool HasReps(this SetMetricTypes setType)
	{
		return setType == SetMetricTypes.WeightReps || setType == SetMetricTypes.RepsOnly;
	}

	public static bool HasSeconds(this SetMetricTypes setType)
	{
		return setType == SetMetricTypes.WeightTime || setType == SetMetricTypes.TimeOnly;
	}

	public static bool HasWeight(this SetMetricTypes setType)
	{
		return setType == SetMetricTypes.WeightReps || setType == SetMetricTypes.WeightTime;
	}
}

