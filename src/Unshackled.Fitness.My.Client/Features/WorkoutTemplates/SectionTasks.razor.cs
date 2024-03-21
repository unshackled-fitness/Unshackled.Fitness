using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates;

public class SectionTasksBase : BaseSectionComponent
{
	[Parameter] public string TemplateSid { get; set; } = string.Empty;
	[Parameter] public WorkoutTaskTypes TaskType { get; set; } = WorkoutTaskTypes.PreWorkout;
	protected bool IsLoading { get; set; }
	protected bool IsEditing { get; set; }
	protected bool IsWorking { get; set; }
	protected bool IsSorting { get; set; } = false;
	protected bool DisableControls => IsLoading || IsWorking || IsSorting;
	protected List<TemplateTaskModel> Tasks { get; set; } = new();
	protected List<FormTemplateTaskModel> EditingTasks { get; set; } = new();
	protected List<FormTemplateTaskModel> DeletedTasks { get; set; } = new();
	protected FormTemplateTaskModel Model { get; set; } = new();
	protected FormTemplateTaskModel.Validator ModelValidator { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		await RefreshTasks();
	}

	protected void HandleBackClicked()
	{
		IsSorting = false;
	}

	protected async Task HandleCancelEditClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected void HandleDeleteClicked(FormTemplateTaskModel task)
	{
		EditingTasks.Remove(task);

		// Track tasks that need to be deleted from database
		if (!string.IsNullOrEmpty(task.Sid))
		{
			DeletedTasks.Add(task);
		}

		// Adjust sort order for remaining sets
		for (int i = 0; i < EditingTasks.Count; i++)
		{
			EditingTasks[i].SortOrder = i;
		}
	}

	protected async Task HandleEditClicked()
	{
		EditingTasks.Clear();
		DeletedTasks.Clear();
		EditingTasks.AddRange(Tasks
			.Select(x => new FormTemplateTaskModel
			{
				DateCreatedUtc = x.DateCreatedUtc,
				DateLastModifiedUtc = x.DateLastModifiedUtc,
				SortOrder = x.SortOrder,
				Text = x.Text,
				Type = x.Type,
				Sid = x.Sid
			})
			.ToList());
		Model = new();
		IsEditing = await UpdateIsEditingSection(true);
	}

	protected void HandleFormSubmitted()
	{
		Model.SortOrder = EditingTasks.Count;
		Model.Type = TaskType;
		EditingTasks.Add(Model);
		Model = new();
	}

	protected void HandleReorderClicked()
	{
		IsSorting = true;
	}

	protected async Task HandleUpdateClicked()
	{
		IsWorking = true;

		if (DeletedTasks.Any() || EditingTasks.Any())
		{
			UpdateTemplateTasksModel model = new()
			{
				 DeletedTasks = DeletedTasks,
				 Tasks = EditingTasks
			};
			var result = await Mediator.Send(new UpdateTemplateTasks.Command(TemplateSid, model));
			ShowNotification(result);

			if (result.Success)
				await RefreshTasks();
		}

		IsSorting = false;
		IsWorking = false;
		IsEditing = await UpdateIsEditingSection(false);
	}

	private async Task RefreshTasks()
	{
		IsLoading = true;
		Tasks = await Mediator.Send(new ListTasks.Query(TemplateSid, TaskType));
		IsLoading = false;
	}
}