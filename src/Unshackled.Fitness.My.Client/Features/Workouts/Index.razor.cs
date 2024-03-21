using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class IndexBase : BaseSearchComponent<SearchWorkoutModel, WorkoutListModel>
{
	protected DateRange DateRangeSearch { get; set; } = new DateRange();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchKey = "SearchWorkouts";

		Breadcrumbs.Add(new BreadcrumbItem("Workouts", null, true));

		SearchModel = await GetLocalSetting(SearchKey) ?? new();
		if (SearchModel.DateStart.HasValue && SearchModel.DateEnd.HasValue)
		{
			DateRangeSearch = new DateRange(SearchModel.DateStart.Value, SearchModel.DateEnd.Value.AddDays(-1));
		}

		await DoSearch(SearchModel.Page);
	}

	protected Color GetStatusColor(WorkoutListModel workout)
	{
		if (workout.DateStartedUtc.HasValue && workout.DateCompletedUtc.HasValue)
			return Color.Secondary;
		else
			return Color.Default;
	}

	protected string GetStatusIcon(WorkoutListModel workout)
	{
		if (workout.DateStartedUtc.HasValue && workout.DateCompletedUtc.HasValue)
			return Icons.Material.Filled.CheckCircle;
		else if (workout.DateStartedUtc.HasValue && !workout.DateCompletedUtc.HasValue)
			return Icons.Material.Filled.Contrast;
		else
			return Icons.Material.Outlined.Circle;
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchWorkouts.Query(SearchModel));
		IsLoading = false;
	}

	protected async Task HandleAddWorkoutClicked()
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddWorkout.Command(string.Empty));
		if (result.Success)
		{
			NavManager.NavigateTo($"/workouts/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
	}

	protected void HandleDateRangeChanged(DateRange dateRange)
	{
		DateRangeSearch = dateRange;
		SearchModel.DateStart = dateRange.Start;
		SearchModel.DateEnd = dateRange.End.HasValue ? dateRange.End.Value.AddDays(1) : dateRange.End;
	}

	protected async override Task HandleResetClicked()
	{
		DateRangeSearch = new DateRange();
		await base.HandleResetClicked();
	}
}