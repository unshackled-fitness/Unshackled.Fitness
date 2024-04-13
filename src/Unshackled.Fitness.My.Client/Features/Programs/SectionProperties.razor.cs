using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Actions;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class SectionPropertiesBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public ProgramModel Program { get; set; } = new();
	[Parameter] public EventCallback<ProgramModel> ProgramChanged { get; set; }

	protected const string FormId = "formProgram";
	protected bool IsEditing { get; set; } = false;
	protected bool IsDuplicating { get; set; } = false;
	protected bool IsSaving { get; set; } = false;
	protected FormUpdateProgramModel Model { get; set; } = new();

	protected bool DisableControls => IsSaving;

	protected string GetSectionTitle()
	{
		if (!IsEditing && !IsDuplicating)
			return Program.Title;
		else if (IsEditing)
			return "Edit Program";
		else
			return "Duplicate Program";
	}

	protected string GetSectionSubTitle()
	{
		if (!IsEditing && !IsDuplicating && Program.DateStarted.HasValue)
			return $"Started: {Program.DateStarted.Value.ToString("D")}";
		else
			return string.Empty;
	}

	protected async Task HandleCancelClicked()
	{
		IsEditing = IsDuplicating = await UpdateIsEditingSection(false);
	}
	
	protected async Task HandleDeleteClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to delete this program? This can not be undone!",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			await UpdateIsEditingSection(true);

			var result = await Mediator.Send(new DeleteProgram.Command(Program.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				NavManager.NavigateTo("/programs");
			}
		}
	}

	protected async Task HandleDuplicateClicked()
	{
		Model = new()
		{
			Sid = Program.Sid,
			Title = Program.Title,
			Description = Program.Description
		};

		IsDuplicating = await UpdateIsEditingSection(true);
	}

	protected async Task HandleDuplicateFormSubmitted(FormUpdateProgramModel model)
	{
		var result = await Mediator.Send(new DuplicateProgram.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/programs/{result.Payload}");
		}
		IsDuplicating = await UpdateIsEditingSection(false);
	}

	protected async Task HandleEditClicked()
	{
		Model = new()
		{
			Title = Program.Title,
			Description = Program.Description,
			Sid = Program.Sid
		};

		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleEditFormSubmitted(FormUpdateProgramModel model)
	{
		IsSaving = true;
		var result = await Mediator.Send(new UpdateProperties.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			await ProgramChanged.InvokeAsync(result.Payload);
		}
		IsSaving = false;
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleStartProgramClicked()
	{
		var options = new DialogOptions { ClassBackground = "bg-blur", MaxWidth = MaxWidth.Medium };

		var parameters = new DialogParameters
		{
			{ nameof(DialogAddToCalendar.ProgramType), Program.ProgramType }
		};

		var dialog = DialogService.Show<DialogAddToCalendar>("Add To Calendar", parameters, options);
		var adding = await dialog.Result;
		if (!adding.Canceled)
		{
			IsSaving = true;
			FormStartProgramModel model = new()
			{
				DateStart = (DateTime)adding.Data,
				Sid = Program.Sid,
				StartingTemplateIndex = Program.StartingTemplate()?.SortOrder ?? 0
			};
			var result = await Mediator.Send(new StartProgram.Command(model));
			if (result.Success)
			{
				Program.DateStarted = model.DateStart;
				Program.NextTemplateIndex = model.StartingTemplateIndex;
				await ProgramChanged.InvokeAsync(Program);
			}
			ShowNotification(result);
			IsSaving = false;
		}
	}

	protected async Task HandleStopProgramClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to remove this program from your calendar?",
				yesText: "Remove", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			IsSaving = true;
			var result = await Mediator.Send(new StopProgram.Command(Program.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				Program.DateStarted = null;
				await ProgramChanged.InvokeAsync(Program);
			}
			IsSaving = false;
		}
	}
}