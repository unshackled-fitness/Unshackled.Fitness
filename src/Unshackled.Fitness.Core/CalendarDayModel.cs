namespace Unshackled.Fitness.Core;

public class CalendarDayModel
{
	public DateOnly Date { get; set; }
	public List<CalendarBlockModel> Blocks { get; set; } = new();
}
