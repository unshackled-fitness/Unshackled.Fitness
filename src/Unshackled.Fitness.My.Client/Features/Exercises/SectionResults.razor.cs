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

	protected ChartState<decimal> ChartBestWeight { get; set; } = new();
	protected ChartState<decimal> ChartHighestVolume { get; set; } = new();
	protected ChartState<int> ChartMostReps { get; set; } = new();
	protected ChartState<int> ChartTime { get; set; } = new();
	protected Dictionary<string, string> CustomLegend { get; set; } = new();

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
		var bestWeightConfig = new BarChart
		{
			LabelAxisX = "Date",
			LabelAxisY = DefaultUnits == UnitSystems.Metric ? "Weight (kg)" : "Weight (lb)",
			Title = "Best Weight"
		};
		ChartBestWeight.Configure("chartBestWeight", bestWeightConfig.Config);

		var highestVolumeConfig = new BarChart
		{
			LabelAxisX = "Date",
			LabelAxisY = DefaultUnits == UnitSystems.Metric ? "Volume (kg)" : "Volume (lb)",
			Title = "Highest Volume"
		};
		ChartHighestVolume.Configure("chartHighestVolume", highestVolumeConfig.Config);

		var mostRepsConfig = new BarChart
		{
			LabelAxisX = "Date",
			LabelAxisY = "Reps",
			Title = "Reps"
		};
		ChartMostReps.Configure("chartMostReps", mostRepsConfig.Config);

		var timeConfig = new BarChart
		{
			LabelAxisX = "Date",
			LabelAxisY = "Time",
			Title = "Time"
		};
		ChartTime.Configure("chartTime", timeConfig.Config);

		if (Exercise.IsTrackingSplit)
		{
			CustomLegend.Add("Left", ChartDataSet.ColorBlue);
			CustomLegend.Add("Right", ChartDataSet.ColorGreen);
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
		FillBestWeightDataSet();
		FillHighestVolumeDataSet();
		FillMostRepsDataSet();
		FillTimesDataSet();
	}

	private void FillBestWeightDataSet()
	{
		if (Exercise.DefaultSetMetricType == SetMetricTypes.WeightReps || Exercise.DefaultSetMetricType == SetMetricTypes.WeightTime)
		{
			List<ChartDataSet<decimal>> dsBestWeight = [];

			ChartDataSet<decimal> dsWeight = new()
			{
				BackgroundColor = ChartDataSet.ColorBlue,
				Label = DefaultUnits == UnitSystems.Metric ? "Weight (kg)" : "Weight (lb)"
			};

			var data = SearchResults.Data
				.Where(x => x.IsBestSetByWeight == true)
				.OrderBy(x => x.DateWorkoutUtc)
				.ToArray();

			foreach (var item in data)
			{
				if (item.WeightKg.HasValue && item.WeightLb.HasValue)
				{
					var pt = new ChartDataPoint<decimal>
					{
						X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
						Y = DefaultUnits == UnitSystems.Metric ? item.WeightKg.Value : item.WeightLb.Value
					};
					dsWeight.Data.Add(pt);
				}
			}
			dsBestWeight.Add(dsWeight);

			ChartBestWeight.LoadData(dsBestWeight.ToArray());
		}
	}

	private void FillHighestVolumeDataSet()
	{
		if (Exercise.DefaultSetMetricType == SetMetricTypes.WeightReps)
		{
			List<ChartDataSet<decimal>> dsHighestVolume = [];

			var data = SearchResults.Data
				.Where(x => x.IsBestSetByVolume == true)
				.OrderBy(x => x.DateWorkoutUtc)
				.ToArray();

			ChartDataSet<decimal> dsVolume = new()
			{
				BackgroundColor = ChartDataSet.ColorBlue,
				Label = DefaultUnits == UnitSystems.Metric ? "Volume (kg)" : "Volume (lb)"
			};

			foreach (var item in data)
			{
				var pt = new ChartDataPoint<decimal>
				{
					X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
					Y = DefaultUnits == UnitSystems.Metric ? item.VolumeKg : item.VolumeLb
				};
				dsVolume.Data.Add(pt);
			}
			dsHighestVolume.Add(dsVolume);

			ChartHighestVolume.LoadData(dsHighestVolume.ToArray());
		}
	}

	private void FillMostRepsDataSet()
	{
		if (Exercise.DefaultSetMetricType == SetMetricTypes.WeightReps || Exercise.DefaultSetMetricType == SetMetricTypes.RepsOnly)
		{
			List<ChartDataSet<int>> dsMostReps = [];

			var data = SearchResults.Data
				.OrderBy(x => x.DateWorkoutUtc)
				.ThenByDescending(x => x.Reps)
				.ThenByDescending(x => x.RepsRight)
				.ThenByDescending(x => x.RepsLeft)
				.ToArray();

			if (Exercise.IsTrackingSplit)
			{
				List<ChartDataSet<int>> leftDataSets = new();
				List<ChartDataSet<int>> rightDataSets = new();

				int dsIndex = 0;
				DateTime currentDate = DateTime.MinValue;
				foreach (var item in data)
				{
					if (item.DateWorkoutUtc > currentDate)
					{
						dsIndex = 0;
						currentDate = item.DateWorkoutUtc;
					}
					else
					{
						dsIndex++;
					}

					if (item.RepsLeft.HasValue)
					{
						if(leftDataSets.Count <= dsIndex)
						{
							ChartDataSet<int> dsRepsLeft = new()
							{
								BackgroundColor = ChartDataSet.ColorBlue,
								Label = $"Left Set {dsIndex + 1}"
							};
							leftDataSets.Add(dsRepsLeft);
						}

						var pt = new ChartDataPoint<int>
						{
							X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
							Y = item.RepsLeft.Value
						};
						leftDataSets[dsIndex].Data.Add(pt);
					}

					if (item.RepsRight.HasValue)
					{
						if (rightDataSets.Count <= dsIndex)
						{
							ChartDataSet<int> dsRepsRight = new()
							{
								BackgroundColor = ChartDataSet.ColorGreen,
								Label = $"Right Set {dsIndex + 1}"
							};
							rightDataSets.Add(dsRepsRight);
						}

						var pt = new ChartDataPoint<int>
						{
							X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
							Y = item.RepsRight.Value
						};
						rightDataSets[dsIndex].Data.Add(pt);
					}
				}

				int maxCount = Math.Max(leftDataSets.Count, rightDataSets.Count);
				for (int i = 0; i < maxCount; i++)
				{
					if (i < leftDataSets.Count)
						dsMostReps.Add(leftDataSets[i]);
					if (i < rightDataSets.Count)
						dsMostReps.Add(rightDataSets[i]);
				}
			}
			else
			{
				List<ChartDataSet<int>> dataSets = new();

				int dsIndex = 0;
				DateTime currentDate = DateTime.MinValue;
				foreach (var item in data)
				{
					if (item.DateWorkoutUtc > currentDate)
					{
						dsIndex = 0;
						currentDate = item.DateWorkoutUtc;
					}
					else
					{
						dsIndex++;
					}

					if (item.Reps.HasValue)
					{
						if (dataSets.Count <= dsIndex)
						{
							ChartDataSet<int> dsReps = new()
							{
								BackgroundColor = ChartDataSet.ColorBlue,
								Label = $"Set {dsIndex + 1}"
							};
							dataSets.Add(dsReps);
						}

						var pt = new ChartDataPoint<int>
						{
							X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
							Y = item.Reps.Value
						};
						dataSets[dsIndex].Data.Add(pt);
					}
				}
				foreach (var ds in dataSets)
				{
					dsMostReps.Add(ds);
				}				
			}

			ChartMostReps.LoadData(dsMostReps.ToArray());
		}
	}

	private void FillTimesDataSet()
	{
		if (Exercise.DefaultSetMetricType == SetMetricTypes.WeightTime || Exercise.DefaultSetMetricType == SetMetricTypes.TimeOnly)
		{
			List<ChartDataSet<int>> dsTimes = [];

			var data = SearchResults.Data
				.OrderBy(x => x.DateWorkoutUtc)
				.ThenByDescending(x => x.Seconds)
				.ThenByDescending(x => x.SecondsRight)
				.ThenByDescending(x => x.SecondsLeft)
				.ToArray();

			if (Exercise.IsTrackingSplit)
			{
				List<ChartDataSet<int>> leftDataSets = new();
				List<ChartDataSet<int>> rightDataSets = new();

				int dsIndex = 0;
				DateTime currentDate = DateTime.MinValue;
				foreach (var item in data)
				{
					if (item.DateWorkoutUtc > currentDate)
					{
						dsIndex = 0;
						currentDate = item.DateWorkoutUtc;
					}
					else
					{
						dsIndex++;
					}

					if (leftDataSets.Count <= dsIndex)
					{
						ChartDataSet<int> dsTimeLeft = new()
						{
							BackgroundColor = ChartDataSet.ColorBlue,
							Label = $"Left Set {dsIndex + 1}"
						};
						leftDataSets.Add(dsTimeLeft);
					}

					var pt = new ChartDataPoint<int>
					{
						X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
						Y = item.SecondsLeft
					};
					leftDataSets[dsIndex].Data.Add(pt);

					if (rightDataSets.Count <= dsIndex)
					{
						ChartDataSet<int> dsRepsRight = new()
						{
							BackgroundColor = ChartDataSet.ColorGreen,
							Label = $"Right Set {dsIndex + 1}"
						};
						rightDataSets.Add(dsRepsRight);
					}

					pt = new ChartDataPoint<int>
					{
						X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
						Y = item.SecondsRight
					};
					rightDataSets[dsIndex].Data.Add(pt);
				}

				int maxCount = Math.Max(leftDataSets.Count, rightDataSets.Count);
				for (int i = 0; i < maxCount; i++)
				{
					if (i < leftDataSets.Count)
						dsTimes.Add(leftDataSets[i]);
					if (i < rightDataSets.Count)
						dsTimes.Add(rightDataSets[i]);
				}
			}
			else
			{
				List<ChartDataSet<int>> dataSets = new();

				int dsIndex = 0;
				DateTime currentDate = DateTime.MinValue;
				foreach (var item in data)
				{
					if (item.DateWorkoutUtc > currentDate)
					{
						dsIndex = 0;
						currentDate = item.DateWorkoutUtc;
					}
					else
					{
						dsIndex++;
					}

					if (dataSets.Count <= dsIndex)
					{
						ChartDataSet<int> dsTime = new()
						{
							BackgroundColor = ChartDataSet.ColorBlue,
							Label = $"Set {dsIndex + 1}"
						};
						dataSets.Add(dsTime);
					}

					var pt = new ChartDataPoint<int>
					{
						X = item.DateWorkoutUtc.ToLocalTime().ToString("MMM dd"),
						Y = item.Seconds
					};
					dataSets[dsIndex].Data.Add(pt);
				}
				foreach (var ds in dataSets)
				{
					dsTimes.Add(ds);
				}
			}

			ChartTime.LoadData(dsTimes.ToArray());
		}
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