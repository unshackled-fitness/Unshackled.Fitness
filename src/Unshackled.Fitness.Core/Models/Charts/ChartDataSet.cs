namespace Unshackled.Fitness.Core.Models.Charts;

public class ChartDataSet<T> where T : struct
{
	public string Label { get; set; } = string.Empty;
	public string BackgroundColor {  get; set; } = string.Empty;
	public string BorderColor { get; set; } = string.Empty;
	public int BorderWidth { get; set; } = 0;
	public List<ChartDataPoint<T>> Data { get; set; } = [];
}

public static class ChartDataSet
{
	public const string ColorBlue = "rgba(82,146,255,0.8)";
	public const string ColorGreen = "rgba(69,151,106,0.8)";
}
