using System.Text.Json.Serialization;

namespace Unshackled.Fitness.Core.Models.Calendars;

public class CalendarBlockModel
{
	public string FilterId { get; set; } = string.Empty;
	public string? Color { get; set; }
	public string? Title { get; set; }
	public bool IsCentered { get; set; }
	public decimal Value { get; set; }

	[JsonIgnore]
	public bool IsVisible { get; set; } = true;
}
