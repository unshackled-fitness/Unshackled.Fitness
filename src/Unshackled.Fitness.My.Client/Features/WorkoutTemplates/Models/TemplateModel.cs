using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class TemplateModel : BaseObject
{
	public long MemberId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public int ExerciseCount { get; set; }
	public string? MusclesTargeted { get; set; }
	public int SetCount { get; set; }
}
