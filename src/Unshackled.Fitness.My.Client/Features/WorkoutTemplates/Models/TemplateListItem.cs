using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class TemplateListItem : BaseObject
{
	public string Title { get; set; } = string.Empty;
	public string? MusclesTargeted { get; set; }
	public int ExerciseCount { get; set; }
	public int SetCount { get; set; }
}
