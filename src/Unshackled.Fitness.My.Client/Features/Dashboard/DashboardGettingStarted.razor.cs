using Unshackled.Fitness.Core.Web.Components;
using Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

namespace Unshackled.Fitness.My.Client.Features.Dashboard;

public class DashboardGettingStartedBase : BaseComponent, IAsyncDisposable
{
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();

		State.OnActiveMemberChange += StateHasChanged;
	}

	public override ValueTask DisposeAsync()
	{
		State.OnActiveMemberChange -= StateHasChanged;
		return base.DisposeAsync();
	}

	protected async Task HandleDismissClicked()
	{
		await Mediator.Send(new HideGettingStarted.Command());
	}
}