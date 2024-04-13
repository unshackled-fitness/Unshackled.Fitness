using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Dashboard.Actions;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard;

public class DashboardStatsBase : BaseComponent
{
	protected DateTime ToDateUtc { get; set; } = DateTimeOffset.Now.Date.AddDays(1).ToUniversalTime();
	protected WorkoutStatsModel Model { get; set; } = new();
	public string LabelYear { get; set; } = string.Empty;
	public bool IsWorking { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		LabelYear = "Past Year";

		// Fill with empty blocks while we load
		Model.ToDateUtc = ToDateUtc;
		Model.Fill();

		await GetStats();
	}

	protected string GetMonthStyle(int start, int end)
	{
		return $"grid-column-start: {start}; grid-column-end: {end};";
	}

	protected string GetDayName(int day)
	{
		return day switch
		{
			1 => "M",
			3 => "W",
			5 => "F",
			_ => string.Empty
		};
	}

	private async Task GetStats()
	{
		// Get data
		var model = await Mediator.Send(new GetWorkoutStats.Query(ToDateUtc));		
		
		// Fill graph
		model.Fill();

		// Replace graph with new
		Model = model;
	}

	protected async Task HandlePastYearClicked()
	{
		LabelYear = "Past Year";
		ToDateUtc = DateTimeOffset.Now.Date.AddDays(1).ToUniversalTime();
		await GetStats();
	}

	protected async Task HandleYearClicked(int year)
	{
		LabelYear = year.ToString();
		ToDateUtc = new DateTime(year + 1, 1, 1).ToUniversalTime();
		await GetStats();
	}
}