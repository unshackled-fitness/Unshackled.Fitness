namespace Unshackled.Fitness.Core.Models.Charts;

public class ChartDataPoint<T> where T : struct
{
	public string X { get; set; } = string.Empty;
	public T Y { get; set; }
}
