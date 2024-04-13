using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.My.Client.Features.Metrics.Actions;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics;

public class SingleBase : BaseComponent
{
	[Parameter] public string Sid { get; set; } = string.Empty;
	protected bool IsLoading { get; set; } = true;
	protected FormMetricDefinitionModel MetricDefinition { get; set; } = new();
	protected CalendarModel CalendarModel { get; set; } = new();
	protected string LabelYear { get; set; } = "Past Year";

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Metrics", "/metrics"));
		Breadcrumbs.Add(new BreadcrumbItem("Metric", null, true));

		LabelYear = "Past Year";

		MetricDefinition = await Mediator.Send(new GetDefinition.Query(Sid));
		IsLoading = false;

		await GetCalendar(DateOnly.FromDateTime(DateTimeOffset.Now.Date));
	}

	private async Task GetCalendar(DateOnly toDate)
	{
		CalendarModel = await Mediator.Send(new GetCalendar.Query(Sid, toDate));
	}

	protected async Task HandlePastYearClicked()
	{
		LabelYear = "Past Year";
		await GetCalendar(DateOnly.FromDateTime(DateTimeOffset.Now.Date));
	}

	protected async Task HandleYearClicked(int year)
	{
		LabelYear = year.ToString();
		await GetCalendar(DateOnly.FromDateTime(new DateTime(year, 12, 31)));
	}
}