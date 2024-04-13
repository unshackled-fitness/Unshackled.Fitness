using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using MudBlazor;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.Core.Components;

public class BaseComponent : ComponentBase, IAsyncDisposable
{
	[CascadingParameter] protected Task<AuthenticationState> AuthState { get; set; } = default!;
	[Inject] IOptionsSnapshot<RemoteAuthenticationOptions<ApiAuthorizationProviderOptions>> OptionsSnapshot { get; set; } = default!;
	[Inject] protected IMediator Mediator { get; set; } = default!;
	[Inject] protected NavigationManager NavManager { get; set; } = default!;
	[Inject] protected ISnackbar Snackbar { get; set; } = default!;
	[Inject] protected AppState State { get; set; } = default!;

	protected bool IsMemberActive { get; set; } = false;

	protected List<BreadcrumbItem> DefaultBreadcrumbs => new()
	{
		new BreadcrumbItem(string.Empty, "/", false, Icons.Material.Filled.Home)
	};

	protected List<BreadcrumbItem> Breadcrumbs { get; set; } = new();

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		// Redirect to login when we get here via a deep link
		var user = (await AuthState).User;
		if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
		{
			NavManager.NavigateToLogin(OptionsSnapshot.Get(Options.DefaultName).AuthenticationPaths.LogInPath);
		}

		Breadcrumbs = DefaultBreadcrumbs;
		IsMemberActive = State.ActiveMember.IsActive;

		State.OnActiveMemberChange += HandleActiveMemberChange;
	}

	public virtual ValueTask DisposeAsync()
	{
		State.OnActiveMemberChange -= HandleActiveMemberChange;
		return ValueTask.CompletedTask;
	}

	protected void HandleActiveMemberChange()
	{
		IsMemberActive = State.ActiveMember.IsActive;
		StateHasChanged();
	}

	protected void ShowNotification(CommandResult? result)
	{
		if (result == null)
			Snackbar.Add(Globals.UnexpectedError, Severity.Error);
		else
			Snackbar.Add(result.Message, result.Success ? Severity.Success : Severity.Error);
	}

	protected void ShowNotification(bool success, string message)
	{
		Snackbar.Add(message, success ? Severity.Success : Severity.Error);
	}
}
