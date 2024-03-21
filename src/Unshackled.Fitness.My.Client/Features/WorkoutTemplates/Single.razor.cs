using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates;

public class SingleBase : BaseComponent
{
	[Parameter] public string TemplateId { get; set; } = string.Empty;
	protected bool IsLoading { get; set; } = true;
	protected bool IsWorking { get; set; } = false;
	protected TemplateModel Template { get; set; } = new();
	protected bool IsEditMode { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool DisableControls => !IsEditMode || IsEditing;
	protected bool DisableTrack => IsEditMode || IsWorking || Template.SetCount == 0;

	protected override async Task OnParametersSetAsync()
	{
		await base.OnParametersSetAsync();

		IsLoading = true;
		Template = await Mediator.Send(new GetTemplate.Query(TemplateId));
		IsLoading = false;
	}

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Templates", "/templates", false));
		Breadcrumbs.Add(new BreadcrumbItem("Template", null, true));
	}

	protected void HandleIsEditingSectionChange(bool editing)
	{
		IsEditing = editing;
	}

	protected void HandleSetsUpdated(List<TemplateSetModel> sets)
	{
		Template.SetCount = sets.Count;
	}

	protected async Task HandleTrackNowClicked()
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddWorkout.Command(Template.Sid));
		if (result.Success)
		{
			NavManager.NavigateTo($"/workouts/{result.Payload}");
		}
		else
		{
			ShowNotification(result);
		}
		IsWorking = false;
	}
}