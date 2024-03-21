namespace Unshackled.Fitness.Core.Enums;

public enum WorkoutSetTypes
{
	Standard = 0,
	Warmup = 1
}

public static class WorkoutSetTypesExtensions
{
	public static string Title(this WorkoutSetTypes workoutSetType)
	{
		return workoutSetType switch
		{
			WorkoutSetTypes.Standard => "Standard",
			WorkoutSetTypes.Warmup => "Warm-up",
			_ => string.Empty,
		};
	}
}