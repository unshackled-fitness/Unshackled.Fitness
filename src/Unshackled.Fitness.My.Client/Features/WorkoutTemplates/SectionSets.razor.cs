using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates;

public class SectionSetsBase : BaseSectionComponent
{
	[Parameter] public string TemplateSid { get; set; } = string.Empty;
	[Parameter] public EventCallback<List<TemplateSetModel>> SetsUpdated { get; set; }

	protected bool IsLoading { get; set; }
	protected List<TemplateSetGroupModel> Groups { get; set; } = new();
	protected List<TemplateSetModel> Sets { get; set; } = new();
	protected List<FormTemplateSetGroupModel> FormGroups { get; set; } = new();
	protected List<FormTemplateSetModel> FormSets { get; set; } = new();
	protected List<FormTemplateSetGroupModel> DeletedGroups { get; set; } = new();
	protected List<FormTemplateSetModel> DeletedSets { get; set; } = new();
	protected bool IsAdding { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool IsSorting { get; set; } = false;
	protected bool IsWorking { get; set; } = false;
	protected bool DisableControls => IsAdding || IsWorking || IsSorting;


	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		await RefreshSets();
	}

	protected async Task HandleAddClicked()
	{
		IsAdding = await UpdateIsEditingSection(true);
	}

	protected void HandleAddDuplicateClicked(FormTemplateSetModel set)
	{
		int idx = FormSets.IndexOf(set) + 1;
		
		var newSet = (FormTemplateSetModel)set.Clone();
		newSet.Sid = string.Empty;
		newSet.SortOrder = idx;
		newSet.IsExpanded = true;
		FormSets.Insert(idx, newSet);

		for (int i = 0; i < FormSets.Count; i++)
		{
			FormSets[i].SortOrder = i;
		}
	}

	protected void HandleAddSets(ExercisePickerResult pickerResult)
	{
		if (pickerResult != null)
		{
			string gSid = FormGroups.LastOrDefault()?.Sid ?? Guid.NewGuid().ToString();
			FormSets.Add(new FormTemplateSetModel()
			{
				Equipment = pickerResult.Equipment,
				SetMetricType = pickerResult.SetMetricType,
				ExerciseSid = pickerResult.ExerciseSid,
				ListGroupSid = gSid,
				IsExpanded = true,
				IsTrackingSplit = pickerResult.IsTrackingSplit,
				Muscles = pickerResult.Muscles,
				RepMode = RepModes.Exact,
				SetType = pickerResult.SetType,
				SortOrder = FormSets.Count,
				Title = pickerResult.Title
			});
		}
		IsAdding = false;
	}

	protected void HandleCancelAddClicked()
	{
		IsAdding = false;
	}

	protected async Task HandleCancelEditClicked()
	{
		IsEditing = await UpdateIsEditingSection(false);
	}

	protected void HandleDeleteClicked(FormTemplateSetModel set)
	{
		FormSets.Remove(set);

		//Store sets with Id for deletion from database
		if (!string.IsNullOrEmpty(set.Sid))
		{
			DeletedSets.Add(set);
		}

		// Adjust sort order for remaining sets
		for (int i = 0; i < FormSets.Count; i++)
		{
			FormSets[i].SortOrder = i;
		}
	}

	protected async Task HandleEditClicked()
	{
		FormSets = Sets
			.Select(x => new FormTemplateSetModel(x))
			.ToList();

		FormGroups = Groups
			.Select(x => new FormTemplateSetGroupModel
			{
				Sid = x.Sid,
				SortOrder = x.SortOrder,
				Title = x.Title
			})
			.ToList();

		DeletedGroups.Clear();
		DeletedSets.Clear();

		IsEditing = await UpdateIsEditingSection(true);
	}

	protected void HandleIsSorting(bool isSorting)
	{
		IsAdding = false;
		IsSorting = isSorting;
		StateHasChanged();
	}

	protected void HandleSortChanged(SortableGroupResult<FormTemplateSetGroupModel, FormTemplateSetModel> result)
	{
		FormGroups = result.Groups;
		FormSets = result.Items;
		DeletedGroups = result.DeletedGroups;
	}

	protected void HandleToggleExpanded(FormTemplateSetModel set)
	{
		set.IsExpanded = !set.IsExpanded;
	}

	protected async Task HandleUpdateClicked()
	{
		IsWorking = true;

		UpdateTemplateSetsModel model = new()
		{
			DeletedGroups = DeletedGroups,
			DeletedSets = DeletedSets,
			Groups = FormGroups,
			Sets = FormSets
		};
		var result = await Mediator.Send(new UpdateTemplateSets.Command(TemplateSid, model));
		ShowNotification(result);

		if (result.Success) {
			await RefreshSets();
			await SetsUpdated.InvokeAsync(Sets);
		}
		
		IsAdding = false;
		IsWorking = false;
		IsEditing = await UpdateIsEditingSection(false);
	}

	private async Task RefreshSets()
	{
		IsLoading = true;
		Groups = await Mediator.Send(new ListSetGroups.Query(TemplateSid));
		Sets = await Mediator.Send(new ListSets.Query(TemplateSid));
		IsLoading = false;
	}

	protected bool ShowTargetReps(FormTemplateSetModel item)
	{
		return item.SetMetricType == SetMetricTypes.WeightReps || item.SetMetricType == SetMetricTypes.RepsOnly;
	}

	protected bool ShowTargetReps(TemplateSetModel item)
	{
		return item.SetMetricType == SetMetricTypes.WeightReps || item.SetMetricType == SetMetricTypes.RepsOnly;
	}
}