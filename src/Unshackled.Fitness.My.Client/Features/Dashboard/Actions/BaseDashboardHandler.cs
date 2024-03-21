namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public abstract class BaseDashboardHandler : BaseHandler
{
	public BaseDashboardHandler(HttpClient httpClient) : base(httpClient, "dashboard") { }
}
