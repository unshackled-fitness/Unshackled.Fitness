namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class UpdateTemplateTasksModel
{
	public List<FormTemplateTaskModel> DeletedTasks { get; set; } = new();
	public List<FormTemplateTaskModel> Tasks { get; set; } = new();
}
