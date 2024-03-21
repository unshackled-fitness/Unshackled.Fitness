using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public partial class ViewLibraryBase : BaseSearchComponent<SearchLibraryModel, LibraryListModel>
{
	[Parameter] public EventCallback OnCancelClicked { get; set; }
	[Parameter] public EventCallback<List<Guid>> OnExercisesSelected { get; set; }
	[Parameter] public SearchLibraryModel? InitialSearch { get; set; }

	protected bool MaxSelectionReached => SelectedSids.Count == AppGlobals.MaxSelectionLimit;
	protected List<Guid> SelectedSids { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		SearchModel = InitialSearch ?? new();

		IsLoading = false;
		await DoSearch(SearchModel.Page);
	}

	public bool DisableCheckbox(Guid matchId)
	{
		return DisableControls
			|| !SelectedSids.Contains(matchId) && MaxSelectionReached;
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		SearchResults = await Mediator.Send(new SearchLibraryExercises.Query(SearchModel));
		IsLoading = false;
	}

	protected async Task HandleAddToMyExercisesClicked()
	{
		if (SelectedSids.Any())
		{
			IsWorking = true;
			await OnExercisesSelected.InvokeAsync(SelectedSids);
			IsWorking = false;
		}
		else
		{
			Snackbar.Add("Nothing selected.", Severity.Error);
		}
	}

	protected async Task HandleCancelClicked()
	{
		await OnCancelClicked.InvokeAsync();
	}

	protected void HandleCheckboxChanged(bool value, Guid sid)
	{
		if (value && !MaxSelectionReached)
			SelectedSids.Add(sid);
		else
			SelectedSids.Remove(sid);
	}

	protected async Task HandleEquipmentTypeChanged(EquipmentTypes value)
	{
		EquipmentTypes prev = SearchModel.EquipmentType;
		SearchModel.EquipmentType = value;
		if (SearchModel.EquipmentType != prev)
			await DoSearch(1);
	}

	protected async Task HandleMuscleTypeChanged(MuscleTypes value)
	{
		MuscleTypes prev = SearchModel.MuscleType;
		SearchModel.MuscleType = value;
		if (SearchModel.MuscleType != prev)
			await DoSearch(1);
	}
}
