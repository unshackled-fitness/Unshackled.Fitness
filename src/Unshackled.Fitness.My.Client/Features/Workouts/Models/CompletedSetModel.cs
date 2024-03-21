using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class CompletedSetModel : BaseObject, IGroupedSortable
{
	public string ListGroupSid { get; set; } = string.Empty;
	public string ExerciseSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int? Reps { get; set; }
	public int? RepsLeft { get; set; }
	public int? RepsRight { get; set; }
	public int Seconds { get; set; }
	public int SecondsLeft { get; set; }
	public int SecondsRight { get; set; }
	public int SecondsTarget { get; set; }
	public decimal WeightLb { get; set; }
	public decimal WeightKg { get; set; }
	public DateTime? DateWorkoutUtc { get; set; }
}

public class CompletedSetGroupModel : ISortableGroup
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }
}
