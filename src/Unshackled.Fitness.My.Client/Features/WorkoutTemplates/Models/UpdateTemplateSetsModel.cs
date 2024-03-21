namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class UpdateTemplateSetsModel
{
	public List<FormTemplateSetGroupModel> DeletedGroups { get; set; } = new();
	public List<FormTemplateSetModel> DeletedSets { get; set; } = new();
	public List<FormTemplateSetGroupModel> Groups { get; set; } = new();
	public List<FormTemplateSetModel> Sets { get; set; } = new();
}
