using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SingleBase : BaseComponent
{
	[Parameter] public string WorkoutSid { get; set; } = string.Empty;

	protected const string FormId = "formWorkout";
	protected FormWorkoutModel Workout { get; set; } = new();
	protected bool IsAddingTemplate { get; set; } = false;
	protected bool IsEditMode { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool IsLoading { get; set; } = true;
	protected bool IsRepeating { get; set; } = false;
	protected bool IsWorking { get; set; }
	protected bool IsWorkoutComplete => Workout.DateStartedUtc.HasValue && Workout.DateCompletedUtc.HasValue;
	protected bool IsWorkoutStarted => Workout.DateStartedUtc.HasValue;
	protected bool DisableControls => !IsEditMode || IsEditing || IsWorking || IsRepeating;
	protected bool DisableWorkoutFinishButton => IsEditMode || IsWorking
		|| Workout.Tasks.Where(x => x.Type == WorkoutTaskTypes.PostWorkout && x.Completed == false).Any();
	protected bool DisableWorkoutStartButton => IsEditMode || IsWorking
		|| Workout.Tasks.Where(x => x.Type == WorkoutTaskTypes.PreWorkout && x.Completed == false).Any();
	protected FormAddTemplateModel AddTemplateModel { get; set; } = new();
	protected List<FormWorkoutTaskModel> PostWorkoutTasks => Workout.Tasks.Where(x => x.Type == WorkoutTaskTypes.PostWorkout).ToList();
	protected List<FormWorkoutTaskModel> PreWorkoutTasks => Workout.Tasks.Where(x => x.Type == WorkoutTaskTypes.PreWorkout).ToList();
	protected bool ShowPostWorkoutTasks { get; set; }
	protected int WorkoutRating { get; set; }
	protected string? WorkoutNotes { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		IsLoading = true;
		Workout = await Mediator.Send(new GetWorkout.Query(WorkoutSid));
		HandleUnrecordedSetsChanged(Workout.Sets.Where(x => x.DateRecordedUtc == null).Any());
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

	protected void HandleCancelAddClicked()
	{
		IsAddingTemplate = false;
	}

	protected void HandleIsEditingSectionChange(bool editing)
	{
		IsEditing = editing;
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

	protected void HandleTaskChanged(FormWorkoutTaskModel task)
	{
		var t = Workout.Tasks.Where(x => x.Sid == task.Sid).SingleOrDefault();
		if (t != null)
		{
			t.Completed = task.Completed;
			StateHasChanged();
		}
	}

	protected void HandleUnrecordedSetsChanged(bool hasUnrecorded)
	{
		if (!hasUnrecorded && IsWorkoutStarted && !IsWorkoutComplete)
		{
			ShowPostWorkoutTasks = true;
		}
		else
		{
			ShowPostWorkoutTasks = false;
		}
	}

	protected async Task HandleWorkoutCompleted()
	{
		IsLoading = true;
		CompleteWorkoutModel model = new()
		{
			Notes = WorkoutNotes,
			Rating = WorkoutRating,
			WorkoutSid = WorkoutSid,
		};
		var result = await Mediator.Send(new CompleteWorkout.Command(model));
		if (result.Success)
		{
			Workout = await Mediator.Send(new GetWorkout.Query(WorkoutSid));
			ShowPostWorkoutTasks = false;
		}
		ShowNotification(result);
		IsLoading = false;
		StateHasChanged();
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
}