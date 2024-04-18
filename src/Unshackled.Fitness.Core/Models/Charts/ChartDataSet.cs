namespace Unshackled.Fitness.Core.Models.Charts;

public class ChartDataSet<T> where T : struct
{
	public string Label { get; set; } = string.Empty;
	public string BackgroundColor {  get; set; } = string.Empty;
	public string BorderColor { get; set; } = string.Empty;
	public List<ChartDataPoint<T>> Data { get; set; } = [];
}

public static class ChartDataSet
{
	public static string[] Colors = [
		"rgb(0,0,255)", // Blue
		"rgb(0,255,0)", // Green
		"rgb(255,99,132)", // Pink
		"rgb(255,182,56)", // Yellow
		"rgb(255,0,0)" // Red
	];
}
