using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SectionTasksBase : BaseSectionComponent
{
	[Parameter] public List<FormWorkoutTaskModel> Tasks { get; set; } = new();
	[Parameter] public WorkoutTaskTypes TaskType { get; set; } = WorkoutTaskTypes.PreWorkout;
	[Parameter] public EventCallback WorkoutStatusChanged { get; set; }

	public List<FormWorkoutTaskModel> FilteredTasks { get; set; } = new();

	protected bool IsWorking { get; set; } = false;

	protected bool DisableControls => IsEditMode || IsWorking;
	protected bool DisableButton => IsEditMode || IsWorking 
		|| FilteredTasks.Where(x => x.Completed == false).Any();

	protected string ButtonText { get; set; } = string.Empty;
	protected string WorkingButtonText { get; set; } = string.Empty;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		ButtonText = TaskType == WorkoutTaskTypes.PreWorkout ? "Start Workout" : "Finish Workout";
		WorkingButtonText = TaskType == WorkoutTaskTypes.PreWorkout ? "Starting" : "Finishing";
		FilteredTasks = Tasks.Where(x => x.Type == TaskType).ToList();
	}

	protected void HandleTaskChecked(FormWorkoutTaskModel task, bool isChecked)
	{
		task.Completed = isChecked;
		StateHasChanged();
	}

	protected async Task HandleButtonClicked()
	{
		IsWorking = true;
		if(WorkoutStatusChanged.HasDelegate)
			await WorkoutStatusChanged.InvokeAsync();
	}
}