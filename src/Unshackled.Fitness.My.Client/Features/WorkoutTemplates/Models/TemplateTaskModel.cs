using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class TemplateTaskModel : BaseObject
{
	public WorkoutTaskTypes Type { get; set; } = WorkoutTaskTypes.PreWorkout;
	public string Text { get; set; } = string.Empty;
	public int SortOrder { get; set; }
}
