namespace Unshackled.Fitness.Core.Models.Charts;

public class BarChart
{
	public string Title { get; set; } = string.Empty;
	public string LabelAxisX { get; set; } = string.Empty;
	public string LabelAxisY { get; set; } = string.Empty;
	public bool ShowLegend { get; set; } = false;

	public object Config => new
	{
		Type = "bar",
		options = new
		{
			responsive = true,
			maintainAspectRatio = true,
			plugins = new { 
				title = new
				{
					display = !string.IsNullOrEmpty(Title),
					text = Title
				},
				legend = new
				{
					display = ShowLegend
				}
			},
			scales = new
			{
				y = new
				{
					beginAtZero = true,
					title = new
					{
						display = !string.IsNullOrEmpty(LabelAxisY),
						text = LabelAxisY
					}
				},
			}
		}
	};
}
