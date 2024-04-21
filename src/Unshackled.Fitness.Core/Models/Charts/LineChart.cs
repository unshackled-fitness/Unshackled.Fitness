namespace Unshackled.Fitness.Core.Models.Charts;

public class LineChart
{
	public string Title { get; set; } = string.Empty;
	public string LabelAxisX { get; set; } = string.Empty;
	public string LabelAxisY { get; set; } = string.Empty;
	public bool ShowLegend { get; set; } = false;

	public object Config => new
	{
		Type = "line",
		options = new
		{
			responsive = true,
			maintainAspectRatio = true,
			plugins = new
			{
				title = new
				{
					display = string.IsNullOrEmpty(Title) ? false : true,
					text = Title
				},
				legend = new
				{
					display = ShowLegend
				}
			},
			scales = new
			{
				x = new
				{
					display = true,
					time = new
					{
						unit = "day",
						displayFormats = new
						{
							day = "MMM DD"
						},
						tooltipFormat = "MMM DD"
					},
					ticks = new 
					{
						source = "data"
					},
					title = new
					{
						display = string.IsNullOrEmpty(LabelAxisX) ? false : true,
						text = LabelAxisX
					},
					type = "time",

				},
				y = new
				{
					beginAtZero = true,
					grace = "10%",
					title = new
					{
						display = string.IsNullOrEmpty(LabelAxisY) ? false : true,
						text = LabelAxisY
					}
				},
			}
		}
	};
}
