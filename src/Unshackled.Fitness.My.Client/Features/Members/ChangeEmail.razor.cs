using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Members.Actions;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members;
public class ChangeEmailBase : BaseComponent
{
	protected FormChangeEmailModel Model { get; set; } = new();
	protected FormChangeEmailModel.Validator ModelValidator { get; set; } = new();

	protected bool IsLoading { get; set; } = true;
	protected bool IsWorking { get; set; }
	protected bool DisableControls => IsWorking;
	protected CurrentUserEmailModel CurrentUserEmail { get; set; } = new();
	protected string? UpdateMessage { get; set; }

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Settings", "/member"));
		Breadcrumbs.Add(new BreadcrumbItem("Change Email", null, true));

		CurrentUserEmail = await Mediator.Send(new GetCurrentUserEmail.Query());
		IsLoading = false;
	}

	protected async Task HandleFormSubmitted()
	{
		Model.BaseUrl = NavManager.ToAbsoluteUri("account/confirm-email-change").AbsoluteUri;

		IsWorking = true;
		var result = await Mediator.Send(new UpdateEmail.Command(Model));
		if(result.Success)
		{
			UpdateMessage = result.Message;
		}
		else
		{
			ShowNotification(result);
		}		
		IsWorking = false;
	}

	protected void HandleFormResetClicked()
	{
		UpdateMessage = null;
		Model = new();
		StateHasChanged();
	}

	protected async Task HandleResendEmailClicked()
	{
		string confirmUrl = NavManager.ToAbsoluteUri("account/confirm-email").AbsoluteUri;

		IsWorking = true;
		var result = await Mediator.Send(new ResendVerificationEmail.Command(confirmUrl));
		ShowNotification(result);
		IsWorking = false;
	}
}