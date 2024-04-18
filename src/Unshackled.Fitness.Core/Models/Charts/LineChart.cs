namespace Unshackled.Fitness.Core.Models.Charts;

public class LineChart
{
	public string Title { get; set; } = string.Empty;
	public string LabelAxisX { get; set; } = string.Empty;
	public string LabelAxisY { get; set; } = string.Empty;

	public object Config => new
	{
		Type = "line",
		options = new
		{
			maintainAspectRatio = true,
			plugins = new { 
				title = new
				{
					display = string.IsNullOrEmpty(Title) ? false : true,
					text = Title
				}
			},
			scales = new
			{
				x = new
				{
					display = true,
					time = new {
						unit = "month",
						displayFormats = new
						{
							day = "YYYY-MM"
						}
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
