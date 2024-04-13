using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public partial class IndexBase : BaseSearchComponent<SearchExerciseModel, ExerciseModel>
{
	protected enum Views
	{
		List,
		Library,
		AddCustom
	}

	[Inject] protected ClientConfiguration ClientConfig { get; set; } = default!;
	[Inject] protected IDialogService DialogService { get; set; } = default!;

	protected const string FormId = "formExercise";
	protected bool MaxSelectionReached => SelectedSids.Count == 2;
	protected List<string> SelectedSids { get; set; } = new();
	protected Views ShowView { get; set; } = Views.List;
	protected SearchLibraryModel? InitialLibrarySearchModel { get; set; }
	protected FormExerciseModel AddModel { get; set; } = new();
	protected bool AllowImport { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();
		SearchKey = "SearchExercises";

		Breadcrumbs.Add(new BreadcrumbItem("Exercises", null, true));
		AllowImport = !string.IsNullOrEmpty(ClientConfig.LibraryApiUrl);

		SearchModel = await GetLocalSetting(SearchKey) ?? new();

		await DoSearch(SearchModel.Page);
	}

	public bool DisableCheckbox(string sid)
	{
		return DisableControls
			|| (!SelectedSids.Contains(sid) && MaxSelectionReached);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		await SaveLocalSetting(SearchKey, SearchModel);
		SearchResults = await Mediator.Send(new SearchExercises.Query(SearchModel));
		IsLoading = false;
	}

	protected void HandleAddCustomClicked()
	{
		AddModel = new()
		{
			Muscles = [MuscleTypes.Abdominals],
			Equipment = [EquipmentTypes.None],
			DefaultSetType = WorkoutSetTypes.Standard,
		};
		ShowView = Views.AddCustom;
	}

	protected async Task HandleAddFormSubmitted(FormExerciseModel model)
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddExercise.Command(model));
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo($"/exercises/{result.Payload}");
		}
		IsWorking = false;
	}

	protected void HandleCancelViewClicked()
	{
		ShowView = Views.List;
	}

	protected void HandleCheckboxChanged(bool value, string sid)
	{
		if (value && !MaxSelectionReached)
			SelectedSids.Add(sid);
		else
			SelectedSids.Remove(sid);
	}

	protected void HandleImportClicked()
	{
		InitialLibrarySearchModel = new()
		{
			EquipmentType = SearchModel.EquipmentType,
			MuscleType = SearchModel.MuscleType,
			Title = SearchModel.Title
		};
		ShowView = Views.Library;
	}

	protected async Task HandleImportExercisesSelected(List<Guid> selectedSids)
	{
		if (!selectedSids.Any())
		{
			ShowNotification(false, "Nothing selected.");
			return;
		}

		IsWorking = true;
		var selected = await Mediator.Send(new SelectLibraryExercises.Query(selectedSids));

		if (selected.Any())
		{
			var result = await Mediator.Send(new ImportExercises.Command(selected));
			ShowNotification(result);
			if (result.Success)
			{
				await DoSearch(SearchModel.Page);
			}
		}
		else
		{
			ShowNotification(false, "Could not retrieve the exercises.");
		}
		IsWorking = false;
		ShowView = Views.List;
		StateHasChanged();
	}

	protected async Task HandleMergeClicked()
	{
		if (MaxSelectionReached)
		{
			DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Small, FullWidth = true };

			var parameters = new DialogParameters();
			parameters.Add(DialogMerge.ParameterSids, SelectedSids);

			var dialog = DialogService.Show<DialogMerge>("Merge Exercises", parameters, options);
			DialogResult confirm = await dialog.Result;

			if (!confirm.Canceled && confirm.Data != null)
			{
				IsWorking = true;
				string? keepId = confirm.Data.ToString();
				if (!string.IsNullOrEmpty(keepId))
				{
					string deleteId = SelectedSids
						.Where(x => x != keepId)
						.First();

					MergeModel merge = new()
					{
						KeptSid = keepId,
						DeletedSid = deleteId
					};

					var result = await Mediator.Send(new MergeExercises.Command(merge));
					if(result.Success)
					{
						await DoSearch(SearchModel.Page);
					}
					SelectedSids.Clear();
					ShowNotification(result);
				}
				IsWorking = false;
			}
		}
	}
}
