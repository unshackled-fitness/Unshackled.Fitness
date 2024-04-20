namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class CompleteWorkoutModel
{
	public string WorkoutSid { get; set; } = string.Empty;
	public int Rating { get; set; }
	public string? Notes { get; set; }
}
