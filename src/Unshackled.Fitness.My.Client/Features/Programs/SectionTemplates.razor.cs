using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Actions;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class SectionTemplatesBase : BaseSectionComponent
{
	[Parameter] public ProgramModel Program { get; set; } = new();
	[Parameter] public EventCallback ProgramUpdated { get; set; }

	protected bool DisableControls => IsSaving;
	protected FormUpdateTemplatesModel FormModel { get; set; } = new();
	protected bool IsEditing { get; set; } = false;
	protected bool IsSaving { get; set; } = false;
	protected List<TemplateListModel> Templates { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		Templates = await Mediator.Send(new ListTemplates.Query());
	}

	protected async Task HandleEditClicked()
	{
		FormModel = new()
		{
			LengthWeeks = Program.LengthWeeks,
			ProgramSid = Program.Sid,
			Templates = Program.Templates
				.Select(x => new FormProgramTemplateModel
				{
					DateCreatedUtc = x.DateCreatedUtc,
					DateLastModifiedUtc = x.DateLastModifiedUtc,
					DayNumber = x.DayNumber,
					IsNew = false,
					MemberSid = x.MemberSid,
					ProgramSid = Program.Sid,
					Sid = x.Sid,
					SortOrder = x.SortOrder,
					WeekNumber = x.WeekNumber,
					WorkoutTemplateName = x.WorkoutTemplateName,
					WorkoutTemplateSid = x.WorkoutTemplateSid
				})
				.ToList()
		};
		IsEditing = await UpdateIsEditingSection(true);
	}

	protected async Task HandleCancelEditClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected async Task HandleSaveClicked()
	{
		IsSaving = true;
		var result = await Mediator.Send(new SaveTemplates.Command(FormModel));
		if (result.Success)
		{
			await ProgramUpdated.InvokeAsync();
		}
		ShowNotification(result);
		IsSaving = false;
		IsEditing = await UpdateIsEditingSection(false);
	}
}