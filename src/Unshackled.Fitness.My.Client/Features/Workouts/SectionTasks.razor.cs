using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SectionTasksBase : BaseSectionComponent
{
	[Parameter] public List<FormWorkoutTaskModel> Tasks { get; set; } = new();
	[Parameter] public string Title { get; set; } = "Checklist";
	[Parameter] public EventCallback<FormWorkoutTaskModel> TaskChanged { get; set; }

	protected bool DisableControls { get; set; }

	protected async Task HandleTaskChecked(FormWorkoutTaskModel task, bool isChecked)
	{
		if (isChecked != task.Completed)
		{
			task.Completed = isChecked;
			DisableControls = true;
			await TaskChanged.InvokeAsync(task);
			DisableControls = false;
			StateHasChanged();
		}
	}
}