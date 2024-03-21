using Microsoft.AspNetCore.Components;
using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Programs.Actions;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs;

public class SingleBase : BaseComponent
{
	[Parameter] public string ProgramSid { get; set; } = string.Empty;
	protected bool IsLoading { get; set; } = true;
	protected bool IsWorking { get; set; } = false;
	protected bool IsEditMode { get; set; } = false;
	protected bool IsEditing { get; set; } = false;
	protected bool IsUpdating { get; set; } = false;
	protected bool DisableControls => !IsEditMode || IsEditing || IsUpdating;
	protected ProgramModel Program { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Programs", "/programs", false));
		Breadcrumbs.Add(new BreadcrumbItem("Program", null, true));

		Program = await Mediator.Send(new GetProgram.Query(ProgramSid));
		IsLoading = false;
	}

	protected void HandleIsEditingSectionChange(bool editing)
	{
		IsEditing = editing;
	}

	protected async Task HandleProgramUpdated()
	{
		IsUpdating = true;
		Program = await Mediator.Send(new GetProgram.Query(ProgramSid));
		IsUpdating = false;
	}
}