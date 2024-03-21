using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Unshackled.Fitness.Core.Web.Components;

public class BaseSectionComponent : ComponentBase, IAsyncDisposable
{
	[Inject] protected IMediator Mediator { get; set; } = default!;
	[Inject] protected ISnackbar Snackbar { get; set; } = default!;
	[Inject] protected NavigationManager NavManager { get; set; } = default!;
	[Inject] protected AppState State { get; set; } = default!;
	[Parameter] public bool IsEditMode { get; set; } = false;
	[Parameter] public bool DisableSectionControls { get; set; } = false;
	[Parameter] public EventCallback<bool> OnIsEditingSectionChange { get; set; }

	protected bool IsMemberActive { get; set; } = false;

	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
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

	protected async Task<bool> UpdateIsEditingSection(bool value)
	{
		await OnIsEditingSectionChange.InvokeAsync(value);
		return value;
	}
}
