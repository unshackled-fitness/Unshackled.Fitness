namespace Unshackled.Fitness.My.Client.Features.Calendar.Models;

public class SearchCalendarModel
{
	public DateOnly ToDate { get; set; }
	public DateOnly FromDate { get; set; }
	public DateTime ToDateUtc { get; set; }
	public DateTime FromDateUtc { get; set; }
	public string? WorkoutColor { get; set; }
}
