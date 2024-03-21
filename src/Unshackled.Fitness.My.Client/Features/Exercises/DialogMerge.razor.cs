using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.My.Client.Features.Exercises.Actions;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises;

public partial class DialogMerge
{
	public const string ParameterSids = "Sids";

	[Inject] protected IMediator Mediator { get; set; } = default!;
	[CascadingParameter] MudDialogInstance Dialog { get; set; } = default!;
	[Parameter] public List<string> Sids { get; set; } = new();

	protected bool IsLoading { get; set; }
	protected List<MergeExerciseModel> Exercises { get; set; } = new();
	protected string SelectedSid { get; set; } = string.Empty;
	protected string? SelectedExercise => Exercises
		.Where(x => x.Sid == SelectedSid)
		.Select(x => x.Title)
		.SingleOrDefault();

	protected string? ConfirmSelected { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		IsLoading = true;
		Exercises = await Mediator.Send(new ListMergeExercises.Query(Sids));
		IsLoading = false;
	}

	protected string? IsMatch(string? input)
	{
		if(!string.IsNullOrEmpty(SelectedExercise) && input != SelectedExercise)
			return "Does not match";
		return null;
	}

	void Submit()
	{
		if (string.IsNullOrEmpty(IsMatch(ConfirmSelected)))
		{
			Dialog.Close(DialogResult.Ok(SelectedSid));
		}
	}
	void Cancel() => Dialog.Cancel();
}