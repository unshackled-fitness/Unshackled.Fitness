using System.Text.Json.Serialization;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Models;

public class MetricModel : BaseMemberObject, IGroupedSortable
{
	public string Title { get; set; } = string.Empty;
	public string? SubTitle { get; set; }
	public MetricTypes MetricType { get; set; }
	public string ListGroupSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public string? HighlightColor { get; set; }
	public decimal MaxValue { get; set; }
	public bool IsArchived { get; set; }
	public DateTime DateRecorded { get; set; }
	public decimal RecordedValue { get; set; }

	[JsonIgnore]
	public bool IsEditing { get; set; }

	[JsonIgnore]
	public bool IsSaving { get; set; }
}
