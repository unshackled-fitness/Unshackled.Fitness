using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Extensions;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public class SectionResultsBase : BaseSearchComponent<SearchResultsModel, ResultListModel>
{
	[Parameter] public ExerciseModel Exercise { get; set; } = new();

	protected DateRange DateRangeSearch { get; set; } = new DateRange();
	protected List<ResultListGroupModel> Groups { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		SearchModel = new()
		{
			ExerciseSid = Exercise.Sid,
			SetMetricType = Exercise.DefaultSetMetricType
		};
		await DoSearch(1);
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

		IsLoading = false;
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
}