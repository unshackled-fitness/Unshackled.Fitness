using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;

public class ExerciseModel : BaseObject
{
	public Guid? MatchId { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes DefaultSetType { get; set; }
	public SetMetricTypes DefaultSetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public string? DefaultMetricsJson { get; set; }
}
