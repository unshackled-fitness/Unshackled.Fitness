using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Extensions;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.Core.Models.Charts;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public class SectionResultsBase : BaseSearchComponent<SearchResultsModel, ResultListModel>
{
	[Parameter] public ExerciseModel Exercise { get; set; } = new();

	protected DateRange DateRangeSearch { get; set; } = new DateRange();
	protected List<ResultListGroupModel> Groups { get; set; } = new();
	protected Views CurrentView { get; set; } = Views.Data;	
	protected UnitSystems DefaultUnits { get; set; } = UnitSystems.Metric;

	protected ChartState<decimal> ChartWeightReps { get; set; } = new();
	protected ChartState<decimal> ChartVolumeReps { get; set; } = new();
	protected ChartState<int> ChartReps { get; set; } = new();
	protected ChartState<decimal> ChartWeightTime { get; set; } = new();
	protected ChartState<decimal> ChartTime { get; set; } = new();

	protected enum Views
	{
		Data,
		Charts
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		DefaultUnits = State.ActiveMember.Settings.DefaultUnits;

		SearchModel = new()
		{
			ExerciseSid = Exercise.Sid,
			SetMetricType = Exercise.DefaultSetMetricType
		};

		ConfigureCharts();

		await DoSearch(1);
	}

	private void ConfigureCharts()
	{
		switch (Exercise.DefaultSetMetricType)
		{
			case SetMetricTypes.WeightReps:
				var weightRepsConfig = new LineChart
				{
					LabelAxisX = "Date",
					LabelAxisY = DefaultUnits == UnitSystems.Metric ? "Weight (kg)" : "Weight (lb)",
					Title = "Best Set By Weight"
				};
				ChartWeightReps.Configure("chartWeightReps", weightRepsConfig.Config);

				var volumeRepsConfig = new LineChart
				{
					LabelAxisX = "Date",
					LabelAxisY = DefaultUnits == UnitSystems.Metric ? "Volume (kg)" : "Volume (lb)",
					Title = "Best Set By Volume"
				};
				ChartVolumeReps.Configure("chartVolumeReps", volumeRepsConfig.Config);
				break;
			case SetMetricTypes.RepsOnly:
				break;
			case SetMetricTypes.WeightTime:
				break;
			case SetMetricTypes.TimeOnly:
				break;
			default:
				break;
		}
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		SearchResults = await Mediator.Send(new SearchResults.Query(SearchModel));
		Groups.Clear();
		string lastSid = string.Empty;
		foreach (var item in SearchResults.Data)
		{
			if (item.ListGroupSid != lastSid)
			{
				Groups.Add(new ResultListGroupModel
				{
					Sid = item.ListGroupSid,
					SortOrder = 0,
					Title = item.DateWorkoutUtc.ToLocalTime().ToString("ddd, MMM dd yyyy"),
				});
				lastSid = item.ListGroupSid;
			}
		}

		FillDataSets();

		IsLoading = false;
	}

	protected void FillDataSets()
	{

		List<ChartDataSet<decimal>> dsBestSets = [];

		var targetReps = SearchResults.Data
			.OrderBy(x => x.RepsTarget)
			.Select(x => x.RepsTarget)
			.Distinct()
			.ToArray();

		int color = 0;
		foreach (int target in targetReps)
		{
			ChartDataSet<decimal> bestSets = new()
			{
				BackgroundColor = ChartDataSet.Colors[color],
				BorderColor = ChartDataSet.Colors[color],
				Label = target == 0 ? "Target: None" : $"Target: {target}"
			};

			var data = SearchResults.Data
				.Where(x => x.IsBestSetByWeight == true && x.RepsTarget == target)
				.OrderBy(x => x.DateWorkoutUtc)
				.ToArray();

			foreach (var item in data)
			{
				if (item.WeightKg.HasValue && item.WeightLb.HasValue)
				{
					var pt = new ChartDataPoint<decimal>
					{
						X = item.DateWorkoutUtc.ToLocalTime().ToString("G"),
						Y = DefaultUnits == UnitSystems.Metric ? item.WeightKg.Value : item.WeightLb.Value
					};
					bestSets.Data.Add(pt);
				}
			}
			dsBestSets.Add(bestSets);

			color++;
			if (color >= ChartDataSet.Colors.Length)
				color = 0;
		}

		ChartWeightReps.LoadData(dsBestSets.ToArray());
	}

	protected Variant GetButtonViewVariant(Views view)
	{
		if (view == CurrentView)
			return Variant.Filled;
		else
			return Variant.Outlined;
	}

	protected decimal GetDisplayWeight(ResultListModel result)
	{
		if (State.ActiveMember.AreDefaultUnits(UnitSystems.Metric))
			return result.VolumeKg;
		else
			return result.VolumeLb;
	}

	protected WeightUnits GetDisplayWeightUnit()
	{
		if (State.ActiveMember.AreDefaultUnits(UnitSystems.Metric))
			return WeightUnits.kg;
		else
			return WeightUnits.lb;
	}

	protected string GetViewClass(Views view)
	{
		if (view == CurrentView)
			return "d-block";
		else
			return "d-none";
	}

	protected void HandleDateRangeChanged(DateRange dateRange)
	{
		DateRangeSearch = dateRange;
		SearchModel.DateStart = dateRange.Start;
		SearchModel.DateEnd = dateRange.End;
	}

	protected async override Task HandleResetClicked()
	{
		DateRangeSearch = new DateRange();
		SearchModel = new()
		{
			ExerciseSid = Exercise.Sid,
			SetMetricType = Exercise.DefaultSetMetricType
		};
		await DoSearch(1);
	}

	protected void HandleViewButtonClicked(Views view)
	{
		CurrentView = view;
		StateHasChanged();
	}
}