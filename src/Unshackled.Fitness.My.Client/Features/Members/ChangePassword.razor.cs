using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Members.Actions;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members;
public class ChangePasswordBase : BaseComponent
{
	protected FormChangePasswordModel Model { get; set; } = new();
	protected FormChangePasswordModel.Validator ModelValidator { get; set; } = new();

	protected bool IsWorking { get; set; }
	protected bool DisableControls => IsWorking;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Settings", "/member"));
		Breadcrumbs.Add(new BreadcrumbItem("Change Email", null, true));
	}

	protected async Task HandleFormSubmitted()
	{
		IsWorking = true;
		var result = await Mediator.Send(new UpdatePassword.Command(Model));
		ShowNotification(result);
		if (result.Success)
		{
			Model = new();
		}
		IsWorking = false;
		StateHasChanged();
	}
}