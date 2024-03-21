using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class TemplateSetModel : BaseObject, IGroupedSortable
{
	public string ExerciseSid { get; set; } = string.Empty;
	public string ListGroupSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public int SortOrder { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int SecondsTarget { get; set; }
	public int IntensityTarget { get; set; }
}
