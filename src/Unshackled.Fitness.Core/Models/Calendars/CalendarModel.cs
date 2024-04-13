namespace Unshackled.Fitness.Core.Models.Calendars;

public class CalendarModel
{
	public DateOnly ToDate { get; set; }
	public DateOnly FromDate { get; set; }
	public List<int> YearsAvailable { get; set; } = new();
	public List<CalendarDayModel> Days { get; set; } = new();
	public List<CalendarBlockFilterGroupModel> BlockFilterGroups { get; set; } = new();
	public List<CalendarBlockFilterModel> BlockFilters { get; set; } = new();

	public bool IsAltDisplay(CalendarDayModel day)
	{
		int startMonth = FromDate.Month;
		int currentMonth = day.Date.Month;

		if (currentMonth == startMonth)
			return false;

		if (currentMonth > startMonth)
			return (currentMonth - startMonth) % 2 != 0;
		else
			return (startMonth - currentMonth) % 2 != 0;
	}
}
