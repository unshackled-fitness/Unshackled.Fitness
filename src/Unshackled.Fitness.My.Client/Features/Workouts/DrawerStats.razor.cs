using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class DrawerStatsBase : BaseSearchComponent<SearchSetModel, CompletedSetModel>
{
	[Parameter] public string ExcludeWorkoutSid { get; set; } = string.Empty;
	[Parameter] public string ExerciseSid { get; set; } = string.Empty;
	[Parameter] public SetMetricTypes SetMetricType { get; set; }
	[Parameter] public RepModes? RepMode { get; set; }
	[Parameter] public int? TargetReps { get; set; }
	[Parameter] public int? SecondsTarget { get; set; }

	protected List<CompletedSetGroupModel> Groups { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		// Set initial values
		SearchModel.ExcludeWorkoutSid = ExcludeWorkoutSid;
		SearchModel.ExerciseSid = ExerciseSid;
		SearchModel.RepMode = RepMode;
		SearchModel.RepsTarget = TargetReps;
		SearchModel.SetMetricType = SetMetricType;
		SearchModel.SecondsTarget = SecondsTarget;

		await DoSearch(1);
	}

	protected override async Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		SearchResults = await Mediator.Send(new SearchCompletedSets.Query(SearchModel));
		Groups.Clear();
		string lastSid = string.Empty;
		foreach (var item in SearchResults.Data)
		{
			if(item.ListGroupSid != lastSid)
			{
				Groups.Add(new CompletedSetGroupModel
				{
					Sid = item.ListGroupSid,
					SortOrder = 0,
					Title = item.DateWorkoutUtc.HasValue ? item.DateWorkoutUtc.Value.ToLocalTime().ToString("d") : string.Empty,
				});
				lastSid = item.ListGroupSid;
			}
		}
		IsLoading = false;
	}

	protected async override Task HandleResetClicked()
	{
		SearchModel = new()
		{
			ExcludeWorkoutSid = ExcludeWorkoutSid,
			ExerciseSid = ExerciseSid,
			SetMetricType = SetMetricType,
		};
		await DoSearch(1);
	}
}