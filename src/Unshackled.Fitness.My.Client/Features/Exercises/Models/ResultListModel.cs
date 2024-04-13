using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class ResultListModel : BaseMemberObject, IGroupedSortable
{
	public DateTime DateWorkoutUtc { get; set; }
	public DateTime DateRecordedUtc { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int? Reps { get; set; }
	public int? RepsLeft { get; set; }
	public int? RepsRight { get; set; }
	public int Seconds { get; set; }
	public int SecondsLeft { get; set; }
	public int SecondsRight { get; set; }
	public int SecondsTarget { get; set; }
	public decimal? WeightLb { get; set; }
	public decimal? WeightKg { get; set; }
	public decimal VolumeKg { get; set; }
	public decimal VolumeLb { get; set; }
	public string ListGroupSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool IsBestSetByWeight { get; set; }
	public bool IsBestSetByVolume { get; set; }
	public bool IsRecordTargetVolume { get; set; }
	public bool IsRecordTargetWeight { get; set; }
	public bool IsRecordVolume { get; set; }
	public bool IsRecordWeight { get; set; }
}

public class ResultListGroupModel : ISortableGroup
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }
}
