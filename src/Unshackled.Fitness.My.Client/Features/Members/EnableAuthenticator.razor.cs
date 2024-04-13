using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Unshackled.Fitness.Core.Components;
using Unshackled.Fitness.My.Client.Features.Members.Actions;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members;
public class EnableAuthenticatorBase : BaseComponent
{
	[Inject] protected IJSRuntime JS { get; set; } = default!;

	protected bool IsLoading { get; set; } = true;
	protected bool IsWorking { get; set; }
	protected bool DisableControls => IsWorking;

	protected AuthenticatorModel AuthenticatorModel { get; set; } = new();
	protected FormAuthenticatorModel FormModel { get; set; } = new();
	protected FormAuthenticatorModel.Validator FormModelValidator { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Settings", "/member"));
		Breadcrumbs.Add(new BreadcrumbItem("Two-factor Authentication", "/member/2fa"));
		Breadcrumbs.Add(new BreadcrumbItem("Configure Authenticator App", null, true));

		AuthenticatorModel = await Mediator.Send(new GetAuthenticatorModel.Query());
		IsLoading = false;
		StateHasChanged();

		if (!string.IsNullOrEmpty(AuthenticatorModel.SharedKey))
		{
			await JS.InvokeAsync<object>(identifier: "CreateQrCode");
		}
	}

	protected async Task HandleFormSubmitted()
	{
		IsWorking = true;
		var result = await Mediator.Send(new AddAuthenticator.Command(FormModel));
		ShowNotification(result);
		if (result.Success)
		{
			if (result.Payload != null)
			{
				AuthenticatorModel.RecoveryCodes = result.Payload;
				StateHasChanged();
			}
			else
			{
				NavManager.NavigateTo("/member/2fa");
			}
		}
		IsWorking = false;
	}
}