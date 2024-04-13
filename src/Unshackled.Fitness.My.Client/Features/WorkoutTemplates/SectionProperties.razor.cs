using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates;

public class SectionPropertiesBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public TemplateModel Template { get; set; } = new();
	[Parameter] public EventCallback<TemplateModel> TemplateChanged { get; set; }

	protected const string FormId = "formWorkoutTemplate";
	protected bool IsEditing { get; set; } = false;
	protected bool IsDuplicating { get; set; } = false;
	protected bool IsSaving { get; set; } = false;
	protected FormTemplateModel Model { get; set; } = new();

	protected bool DisableControls => IsSaving;

	protected string GetSectionTitle()
	{
		if (!IsEditing && !IsDuplicating)
			return Template.Title;
		else if (IsEditing)
			return "Edit Workout Template";
		else
			return "Duplicate Workout Template";
	}

	protected async Task HandleCancelClicked()
	{
		IsEditing = IsDuplicating = await UpdateIsEditingSection(false);
	}
	
	protected async Task HandleDeleteClicked()
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to delete this template? This can not be undone!",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			await UpdateIsEditingSection(true);

			var result = await Mediator.Send(new DeleteTemplate.Command(Template.Sid));
			ShowNotification(result);
			if (result.Success)
			{
				NavManager.NavigateTo("/templates");
			}
		}
	}

	protected async Task HandleDuplicateClicked()
	{
		Model = new()
		{
			Title = Template.Title,
			Description = Template.Description
		};

		IsDuplicating = await UpdateIsEditingSection(true);
	}

	protected async Task HandleDuplicateFormSubmitted(FormTemplateModel model)
	{
		var result = await Mediator.Send(new DuplicateTemplate.Command(Template.Sid, model));
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/templates/{result.Payload}");
		}
		IsDuplicating = await UpdateIsEditingSection(false);
	}

	protected async Task HandleEditClicked()
	{
		Model = new()
		{
			Title = Template.Title,
			Description = Template.Description,
			Sid = Template.Sid
		};

		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleEditFormSubmitted(FormTemplateModel model)
	{
		IsSaving = true;
		var result = await Mediator.Send(new UpdateTemplateProperties.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			if (TemplateChanged.HasDelegate)
				await TemplateChanged.InvokeAsync(result.Payload);
		}
		IsSaving = false;
		IsEditing = await UpdateIsEditingSection(false);
	}
}