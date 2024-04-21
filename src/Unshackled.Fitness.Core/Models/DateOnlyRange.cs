namespace Unshackled.Fitness.Core.Models;

public class DateOnlyRange
{
	public DateOnly Start { get; set; }
	public DateOnly End { get; set; }

	public DateOnlyRange() { }
	public DateOnlyRange(DateOnly start, DateOnly end)
	{
		Start = start;
		End = end;
	}
}
