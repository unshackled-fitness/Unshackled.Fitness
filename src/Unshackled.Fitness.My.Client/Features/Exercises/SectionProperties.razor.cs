using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public class SectionPropertiesBase : BaseSectionComponent
{
	[Inject] protected ClientConfiguration ClientConfig { get; set; } = default!;
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public ExerciseModel Exercise { get; set; } = new();
	[Parameter] public EventCallback<ExerciseModel> ExerciseChanged { get; set; }

	protected const string FormId = "formExercise";
	protected bool DrawerOpen { get; set; }
	protected bool IsEditing { get; set; } = false;
	protected bool IsUpdating { get; set; } = false;
	protected FormExerciseModel Model { get; set; } = new();
	protected ExerciseNoteModel FormNoteModel { get; set; } = new();
	public bool AllowRestore { get; set; }

	public bool DisableControls => IsUpdating;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		AllowRestore = !string.IsNullOrEmpty(ClientConfig.LibraryApiUrl);
	}


	protected async Task HandleEditClicked()
	{
		Model = new()
		{
			DefaultSetMetricType = Exercise.DefaultSetMetricType,
			Description = Exercise.Description,
			Equipment = Exercise.Equipment,
			IsTrackingSplit = Exercise.IsTrackingSplit,
			Muscles = Exercise.Muscles,
			Title = Exercise.Title,
			Sid = Exercise.Sid
		};

		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleEditCancelClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected void HandleEditNoteClicked()
	{
		FormNoteModel = new()
		{
			Sid = Exercise.Sid,
			MemberSid = Exercise.MemberSid,
			Notes = Exercise.Notes
		};
		DrawerOpen = true;
	}

	protected async Task HandleFormSubmitted(FormExerciseModel model)
	{
		IsUpdating = true;
		var result = await Mediator.Send(new UpdateExercise.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			if (ExerciseChanged.HasDelegate)
				await ExerciseChanged.InvokeAsync(result.Payload);
		}
		IsUpdating = false;
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleRestoreClicked()
	{
		if (Exercise.MatchId.HasValue)
		{
			bool? confirm = await DialogService.ShowMessageBox(
					"Warning",
					"Are you sure you want to restore this exercise? This will overwrite all properties with their original values.",
					yesText: "Restore", cancelText: "Cancel");

			if (confirm.HasValue && confirm.Value)
			{
				IsUpdating = await UpdateIsEditingSection(true);
				var libExercise = await Mediator.Send(new RestoreExercise.Command(Exercise.MatchId.Value));
				if (libExercise != null)
				{
					FormExerciseModel model = new()
					{
						DefaultSetMetricType = libExercise.DefaultSetMetricType,
						DefaultSetType = libExercise.DefaultSetType,
						Description = libExercise.Description,
						Equipment = libExercise.Equipment,
						IsTrackingSplit = libExercise.IsTrackingSplit,
						Muscles = libExercise.Muscles,
						Sid = Exercise.Sid,
						Title = libExercise.Title
					};

					var result = await Mediator.Send(new UpdateExercise.Command(model));
					if (result.Success && result.Payload != null)
					{
						if (ExerciseChanged.HasDelegate)
							await ExerciseChanged.InvokeAsync(result.Payload);
					}
					ShowNotification(result);
				}
				else
				{
					ShowNotification(false, "Exercise not found in library.");
				}
				IsUpdating = await UpdateIsEditingSection(false);
			}
		}
	}

	protected async void HandleSaveNoteClicked()
	{
		IsUpdating = true;
		var result = await Mediator.Send(new SaveExerciseNote.Command(FormNoteModel));
		if (result.Success)
		{
			Exercise.Notes = FormNoteModel.Notes;
			DrawerOpen = false;
			await ExerciseChanged.InvokeAsync(Exercise);
		}
		ShowNotification(result);
		IsUpdating = false;
	}

	protected async Task HandleToggleArchiveClicked()
	{
		IsUpdating = true;
		var result = await Mediator.Send(new ToggleIsArchived.Command(Exercise.Sid));
		if (result.Success)
		{
			Exercise.IsArchived = result.Payload;
		}
		ShowNotification(result);
		IsUpdating = false;
	}
}