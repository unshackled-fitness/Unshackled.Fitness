using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Workouts.Actions;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts;

public class SectionPropertiesBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public FormWorkoutModel Workout { get; set; } = new();
	[Parameter] public EventCallback<FormWorkoutModel> WorkoutChanged { get; set; }

	protected const string FormId = "formWorkout";
	protected bool IsEditing { get; set; } = false;
	protected bool DisableControls { get; set; } = false;
	protected FormPropertiesModel Model { get; set; } = new();

	protected async Task HandleDeleteClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to delete this workout? This can not be undone!",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			await UpdateIsEditingSection(true);

			var result = await Mediator.Send(new DeleteWorkout.Command(Workout.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				NavManager.NavigateTo("/workouts");
			}
		}
	}

	protected async Task HandleEditClicked()
	{
		Model = new()
		{
			DateCompletedUtc = Workout.DateCompletedUtc,
			DateStartedUtc = Workout.DateStartedUtc,
			IsComplete = Workout.DateCompletedUtc.HasValue,
			IsStarted = Workout.DateStartedUtc.HasValue,
			Notes = Workout.Notes,
			Rating = Workout.Rating,
			Title = Workout.Title,
			Sid = Workout.Sid
		};

		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleEditCancelClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleEditFormSubmitted(FormPropertiesModel model)
	{
		IsEditing = false;
		DisableControls = true;
		var result = await Mediator.Send(new UpdateProperties.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			Workout.Title = model.Title;
			Workout.DateStartedUtc = model.DateStartedUtc;
			Workout.DateCompletedUtc = model.DateCompletedUtc;
			Workout.Rating = model.Rating;
			Workout.Notes = model.Notes;
			if (WorkoutChanged.HasDelegate)
				await WorkoutChanged.InvokeAsync(Workout);
		}
		DisableControls = false;
		await UpdateIsEditingSection(false);
	}
}