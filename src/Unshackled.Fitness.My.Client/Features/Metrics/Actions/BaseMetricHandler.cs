namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public abstract class BaseMetricHandler : BaseHandler
{
	public BaseMetricHandler(HttpClient httpClient) : base(httpClient, "metrics") { }
}
