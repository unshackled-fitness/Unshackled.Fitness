using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class ViewFixedRepeatingBase : BaseSectionComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public FormUpdateTemplatesModel FormModel { get; set; } = new();
	[Parameter] public EventCallback<FormUpdateTemplatesModel> FormModelChanged { get; set; }
	[Parameter] public bool IsEditing { get; set; } = false;
	[Parameter] public bool IsSaving { get; set; } = false;
	[Parameter] public ProgramModel Program { get; set; } = new();
	[Parameter] public List<TemplateListModel> Templates { get; set; } = new();

	protected List<ProgramTemplateModel> GetDayTemplates(int week, int day)
	{
		return Program.Templates
			.Where(x => x.WeekNumber == week && x.DayNumber == day)
			.OrderBy(x => x.SortOrder)
			.ToList();
	}

	protected List<FormProgramTemplateModel> GetFormDayTemplates(int week, int day)
	{
		return FormModel.Templates
			.Where(x => x.WeekNumber == week && x.DayNumber == day)
			.OrderBy(x => x.SortOrder)
			.ToList();
	}

	protected async Task HandleAddTemplateClicked(int week, int day)
	{
		var parameters = new DialogParameters
		{
			{ nameof(DialogAddTemplate.Templates), Templates }
		};

		var options = new DialogOptions { ClassBackground = "bg-blur", MaxWidth = MaxWidth.Medium };

		var dialog = DialogService.Show<DialogAddTemplate>("Add Template", parameters, options);
		var result = await dialog.Result;
		if (!result.Canceled)
		{
			var model = (TemplateListModel)result.Data;
			if (model != null)
			{
				int sortOrder = FormModel.Templates
					.Where(x => x.WeekNumber == week && x.DayNumber == day)
					.OrderBy(x => x.SortOrder)
					.Select(x => x.SortOrder + 1)
					.LastOrDefault();

				FormModel.Templates.Add(new()
				{
					DayNumber = day,
					IsNew = true,
					MemberSid = Program.MemberSid,
					ProgramSid = Program.Sid,
					Sid = Guid.NewGuid().ToString(),
					SortOrder = sortOrder,
					WeekNumber = week,
					WorkoutTemplateSid = model.Sid,
					WorkoutTemplateName = model.Title
				});

				FormModel.Templates = ReorderTemplates();
				await FormModelChanged.InvokeAsync(FormModel);
			}
		}
	}

	protected async Task HandleAddWeekClicked()
	{
		FormModel.LengthWeeks++;
		await FormModelChanged.InvokeAsync(FormModel);
	}

	protected async Task HandleDeleteTemplateClicked(string sid)
	{
		var model = FormModel.Templates
			.Where(x => x.Sid == sid)
			.SingleOrDefault();

		if (model != null)
		{
			if (!model.IsNew)
			{
				FormModel.DeletedTemplates.Add(model);
			}
			FormModel.Templates.Remove(model);
			FormModel.Templates = ReorderTemplates();
			await FormModelChanged.InvokeAsync(FormModel);
		}			
	}

	protected async Task HandleDeleteWeekClicked(int week)
	{
		bool? confirm = await DialogService.ShowMessageBox(
				"Warning",
				"Are you sure you want to delete this week?",
				yesText: "Delete", cancelText: "Cancel");

		if (confirm.HasValue && confirm.Value)
		{
			var list = FormModel.Templates.ToArray();
			foreach (var model in list)
			{
				if (model.WeekNumber == week)
				{
					if (!model.IsNew)
					{
						FormModel.DeletedTemplates.Add(model);
					}
					FormModel.Templates.Remove(model);
				}
				else if (model.WeekNumber > week)
				{
					model.WeekNumber--;
				}
			}
			list = null;
			FormModel.LengthWeeks--;
			await FormModelChanged.InvokeAsync(FormModel);
		}
	}

	private List<FormProgramTemplateModel> ReorderTemplates()
	{
		var list = FormModel.Templates
			.OrderBy(x => x.WeekNumber)
				.ThenBy(x => x.DayNumber)
				.ThenBy(x => x.SortOrder)
			.ToList();

		for (int i = 0; i < list.Count; i++)
		{
			list[i].SortOrder = i;
		}

		return list;
	}
}