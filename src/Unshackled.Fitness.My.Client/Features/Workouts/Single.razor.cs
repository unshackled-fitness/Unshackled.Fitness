using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SingleBase : BaseComponent
{
	[Parameter] public string WorkoutSid { get; set; } = string.Empty;

	protected const string FormId = "formWorkout";
	protected bool IsLoading { get; set; } = true;
	protected bool IsWorking { get; set; }
	protected FormWorkoutModel Workout { get; set; } = new();
	protected bool IsEditMode { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool IsRepeating { get; set; } = false;
	protected bool IsAddingTemplate { get; set; } = false;
	protected bool DisableControls => !IsEditMode || IsEditing || IsWorking || IsRepeating;
	protected FormAddTemplateModel AddTemplateModel { get; set; } = new();	

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		IsLoading = true;
		Workout = await Mediator.Send(new GetWorkout.Query(WorkoutSid));
		IsLoading = false;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Workouts", "/workouts", false));
		Breadcrumbs.Add(new BreadcrumbItem("Workout", null, true));
	}

	protected void HandleAddTemplateClicked()
	{
		AddTemplateModel = new()
		{
			WorkoutSid = WorkoutSid
		};
		IsAddingTemplate = true;
	}

	protected void HandleCancelAddClicked() 
	{
		IsAddingTemplate = false;
	}

	protected void HandleIsEditingSectionChange(bool editing)
	{
		IsEditing = editing;
	}

	protected async Task HandleAddTemplateSubmitted(FormAddTemplateModel model)
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddTemplate.Command(model));
		if (result.Success)
		{
			NavManager.NavigateTo($"/templates/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		IsAddingTemplate = false;
		IsWorking = false;
	}

	protected async Task HandleRepeatWorkoutClicked()
	{
		IsRepeating = true;
		var result = await Mediator.Send(new AddWorkout.Command(WorkoutSid));
		if (result.Success)
		{
			NavManager.NavigateTo($"/workouts/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		IsRepeating = false;
	}

	protected async Task HandleSetSaved()
	{
		if (Workout.DateCompletedUtc.HasValue)
		{
			IsLoading = true;
			Workout = await Mediator.Send(new GetWorkout.Query(WorkoutSid));
			IsLoading = false;
		}
	}

	protected async Task HandleWorkoutStarted()
	{
		IsWorking = true;
		var result = await Mediator.Send(new StartWorkout.Command(WorkoutSid));
		if (result.Success)
		{
			Workout.DateStartedUtc = result.Payload;
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
	}

	protected async Task HandleWorkoutCompleted()
	{
		IsLoading = true;
		var result = await Mediator.Send(new CompleteWorkout.Command(WorkoutSid));
		if (result.Success)
		{
			Workout = await Mediator.Send(new GetWorkout.Query(WorkoutSid));
		}
		ShowNotification(result);
		IsLoading = false;
	}
}