using System.Text.Json.Serialization;

namespace Unshackled.Fitness.Core.Models.Calendars;

public class CalendarBlockFilterModel : IGroupedSortable
{
	public string FilterId { get; set; } = string.Empty;
	public string ListGroupSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string? SubTitle { get; set; }
	public string? Color { get; set; }
	public int SortOrder { get; set; }

	[JsonIgnore]
	public bool IsChecked { get; set; } = true;
}
