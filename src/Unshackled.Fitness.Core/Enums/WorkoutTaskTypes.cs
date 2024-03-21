namespace Unshackled.Fitness.Core.Enums;

public enum WorkoutTaskTypes
{
	PreWorkout,
	PostWorkout
}

public static class ToDoTypesExtensions
{
	public static string Title(this WorkoutTaskTypes type)
	{
		return type switch
		{
			WorkoutTaskTypes.PreWorkout => "Pre-workout",
			WorkoutTaskTypes.PostWorkout => "Post-workout",
			_ => string.Empty,
		};
	}
}