using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public class SingleBase : BaseComponent
{
	[Inject] protected IDialogService DialogService { get; set; } = default!;
	[Parameter] public string Sid { get; set; } = string.Empty;
	protected bool IsLoading { get; set; } = true;
	protected bool IsEditMode { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool DisableControls => !IsEditMode || IsEditing;
	protected ExerciseModel Exercise { get; set; } = new();
	protected FormExerciseModel FormModel { get; set; } = new();
	protected FormExerciseModel.Validator FormModelValidator { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Exercises", "/exercises"));
		Breadcrumbs.Add(new BreadcrumbItem("Exercise", null, true));

		Exercise = await Mediator.Send(new GetExercise.Query(Sid));
		IsLoading = false;
	}

	protected void HandleIsEditingChange(bool editing)
	{
		IsEditing = editing;
	}
}