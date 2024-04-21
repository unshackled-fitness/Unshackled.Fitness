using System.Text.Json.Serialization;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class SearchResultsModel : SearchModel
{
	public SearchResultsModel()
	{
		PageSize = 30;
	}

	public string ExerciseSid { get; set; } = string.Empty;
	public DateTime? DateStart { get; set; }
	public DateTime? DateEnd { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public WorkoutSetTypes SetType { get; set; } = WorkoutSetTypes.Standard;
	public int? RepsTarget { get; set; }
	public int? SecondsTarget { get; set; }

	[JsonIgnore]
	public TimeSpan? TimeSeconds
	{
		get => SecondsTarget.HasValue && SecondsTarget > 0 ? new(0, 0, SecondsTarget.Value) : null;
		set
		{
			SecondsTarget = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : null;
		}
	}
}
