using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Dashboard.Actions;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard;

public class DashboardMetricsBase : BaseComponent
{
	protected DateTime? DisplayDate { get; set; } = DateTimeOffset.Now.Date;
	protected bool IsLoading { get; set; } = true;
	protected MetricGridModel GridModel { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await GetMetrics();
	}

	private async Task GetMetrics()
	{
		if (DisplayDate.HasValue)
		{
			IsLoading = true;
			GridModel = await Mediator.Send(new ListMetrics.Query(DisplayDate.Value));
			IsLoading = false;
		}
	}

	protected async Task HandleDateChanged(DateTime? date)
	{
		if (date.HasValue)
		{
			DisplayDate = new DateTimeOffset(date.Value).Date;
			await GetMetrics();
		}
	}

	protected async Task HandleSaveMetric(MetricModel model, decimal value)
	{
		if (DisplayDate.HasValue)
		{
			model.IsSaving = true;
			SaveMetricModel saveModel = new()
			{
				DefinitionSid = model.Sid,
				RecordedDate = DateOnly.FromDateTime(DisplayDate.Value),
				RecordedValue = value
			};

			var result = await Mediator.Send(new SaveMetric.Command(saveModel));
			ShowNotification(result);
			if (result.Success)
			{
				model.RecordedValue = value;
			}
			model.IsSaving = false;
		}
	}
}