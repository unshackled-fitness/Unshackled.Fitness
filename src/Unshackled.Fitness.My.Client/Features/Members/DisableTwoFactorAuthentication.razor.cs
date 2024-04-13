using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Members.Actions;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members;

public class DisableTwoFactorAuthenticationBase : BaseComponent
{
	protected bool IsWorking { get; set; }
	protected bool DisableControls => IsWorking;
	protected RecoveryCodesModel Model { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Settings", "/member"));
		Breadcrumbs.Add(new BreadcrumbItem("Disable 2FA", null, true));
	}

	protected async Task HandleDisableClicked()
	{
		IsWorking = true;
		var result = await Mediator.Send(new Disable2fa.Command());
		ShowNotification(result);
		if (result.Success)
		{
			NavManager.NavigateTo("/member/2fa");
		}
		IsWorking = false;
	}
}