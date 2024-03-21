using MudBlazor;
using Unshackled.Fitness.Core.Web.Components;

namespace Unshackled.Fitness.My.Client.Features.Members;

public partial class IndexBase : BaseComponent
{
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		Breadcrumbs.Add(new BreadcrumbItem("Settings", null, true));
	}
}
