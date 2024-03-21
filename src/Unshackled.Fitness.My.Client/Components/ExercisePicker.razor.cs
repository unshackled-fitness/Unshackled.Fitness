using Microsoft.AspNetCore.Components;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.ExercisePicker.Actions;
using Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;

namespace Unshackled.Fitness.Core.Web.Components;

public class ExercisePickerBase : BaseSearchComponent<SearchExerciseModel, ExerciseModel>
{
	[Parameter] public EventCallback<ExercisePickerResult> OnAdd { get; set; }

	protected ExercisePickerResult? SelectedExercise { get; set; }

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		await DoSearch(1);
	}

	protected async override Task DoSearch(int page)
	{
		SearchModel.Page = page;

		IsLoading = true;
		SearchResults = await Mediator.Send(new SearchExercises.Query(SearchModel));
		IsLoading = false;
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

	protected async Task HandleSelectExerciseClicked(ExerciseModel model)
	{
		SelectedExercise = new()
		{
			ExerciseSid = model.Sid,
			SetType = model.DefaultSetType,
			Title = model.Title,
			IsTrackingSplit = model.IsTrackingSplit,
			Muscles = model.Muscles,
			Equipment = model.Equipment,
			SetMetricType = model.DefaultSetMetricType
		};
		if (OnAdd.HasDelegate)
			await OnAdd.InvokeAsync(SelectedExercise);
	}
}

public class ExercisePickerResult
{
	public string ExerciseSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
}