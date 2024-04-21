using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.Core.Models.Charts;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Metrics.Actions;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using static MudBlazor.CategoryTypes;

namespace Unshackled.Fitness.My.Client.Features.Metrics;

public class SingleBase : BaseComponent
{
	[Inject] protected ILocalStorage localStorageService { get; set; } = default!;
	[Parameter] public string Sid { get; set; } = string.Empty;
	protected bool IsLoading { get; set; } = true;
	protected bool DisableControls => IsLoading;
	protected FormMetricDefinitionModel MetricDefinition { get; set; } = new();
	protected CalendarModel CalendarModel { get; set; } = new();
	protected SearchCalendarModel SearchModel { get; set; } = new();
	protected FormSearchModel FormModel { get; set; } = new();
	protected ChartState<decimal> ChartMetric { get; set; } = new();
	protected Views CurrentView { get; set; } = Views.Calendar;

	private string searchKey = "SearchMetrics";
	private DateTime defaultDate = new DateTime(DateTimeOffset.Now.Year, DateTimeOffset.Now.Month, 1);
	protected enum Views
	{
		Calendar,
		Charts
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Metrics", "/metrics"));
		Breadcrumbs.Add(new BreadcrumbItem("Metric", null, true));

		await InitializeSearchForm();
		ConfigureCharts();

		MetricDefinition = await Mediator.Send(new GetDefinition.Query(Sid));

		await GetCalendar();
		IsLoading = false;
	}

	public void ConfigureCharts()
	{
		var metricConfig = new LineChart();
		ChartMetric.Configure("chartMetric", metricConfig.Config);
	}

	private void FillMetricDataSet()
	{
		List<ChartDataSet<decimal>> dsMetrics = [];

		ChartDataSet<decimal> dsMetric = new()
		{
			BackgroundColor = ChartDataSet.ColorBlue,
			BorderColor = ChartDataSet.ColorBlue,
			BorderWidth = 1,
		};

		foreach (var day in CalendarModel.Days)
		{
			if (day != null)
			{
				string dateLabel = day.Date.ToString("MMM dd");

				foreach (var block in day.Blocks)
				{
					var pt = new ChartDataPoint<decimal>
					{
						X = dateLabel,
						Y = block.Value
					};
					dsMetric.Data.Add(pt);
				}
				
			}
		}
		dsMetrics.Add(dsMetric);

		ChartMetric.LoadData(dsMetrics.ToArray());
	}

	protected Variant GetButtonViewVariant(Views view)
	{
		if (view == CurrentView)
			return Variant.Filled;
		else
			return Variant.Outlined;
	}

	private async Task GetCalendar()
	{
		SearchModel.FromDateUtc = SearchModel.FromDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local).ToUniversalTime();
		SearchModel.ToDateUtc = SearchModel.ToDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local).ToUniversalTime();

		CalendarModel = await Mediator.Send(new GetCalendar.Query(Sid, SearchModel));

		FillMetricDataSet();
	}

	protected string GetMonthDisplay(int num)
	{
		string output = "--";
		if (FormModel.EndDate.HasValue && num > 0)
		{
			string title = num == 1 ? "month" : "months";
			output = $"{num} {title} ({FormModel.EndDate.Value.AddMonths(-num).ToString("MM/yyyy")})";
		}
		return output;
	}

	protected string GetViewClass(Views view)
	{
		if (view == CurrentView)
			return "d-block";
		else
			return "d-none";
	}

	protected async Task HandleResetClicked()
	{
		await InitializeSearchForm(true);
		await localStorageService.RemoveItemAsync(searchKey);
		await GetCalendar();
	}

	protected async Task HandleSearchClicked()
	{
		var range = Calculator.DateRange(FormModel.EndDate, FormModel.NumberOfMonths, defaultDate);
		SearchModel = new()
		{
			FromDate = range.Start,
			ToDate = range.End
		};
		await localStorageService.SetItemAsync(searchKey, FormModel, CancellationToken.None);
		await GetCalendar();
	}

	protected void HandleViewButtonClicked(Views view)
	{
		CurrentView = view;
		StateHasChanged();
	}

	private async Task InitializeSearchForm(bool reset = false)
	{
		var defaultModel = new FormSearchModel
		{
			EndDate = new DateTime(DateTimeOffset.Now.Date.Year, DateTimeOffset.Now.Date.Month, 1),
			NumberOfMonths = 0
		};

		FormModel = reset ? defaultModel : await localStorageService.GetItemAsync<FormSearchModel>(searchKey) ?? defaultModel;

		var range = Calculator.DateRange(FormModel.EndDate, FormModel.NumberOfMonths, defaultDate);
		SearchModel = new()
		{
			FromDate = range.Start,
			ToDate = range.End
		};
	}
}