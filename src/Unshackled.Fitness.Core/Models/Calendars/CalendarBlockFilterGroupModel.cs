using System.Text.Json.Serialization;

namespace Unshackled.Fitness.Core.Models.Calendars;

public class CalendarBlockFilterGroupModel : ISortableGroup
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }

	[JsonIgnore]
	public bool? AllCheckedState { get; set; }
}
