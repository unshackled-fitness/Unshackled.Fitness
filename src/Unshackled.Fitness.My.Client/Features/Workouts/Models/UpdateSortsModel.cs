namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class UpdateSortsModel
{
	public string WorkoutSid { get; set; } = string.Empty;
	public List<FormWorkoutSetGroupModel> DeletedGroups { get; set; } = new();
	public List<FormWorkoutSetGroupModel> Groups { get; set; } = new();
	public List<FormWorkoutSetModel> Sets { get; set; } = new();
}
