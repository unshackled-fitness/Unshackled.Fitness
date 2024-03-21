namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public abstract class BaseMemberHandler : BaseHandler
{
	public BaseMemberHandler(HttpClient httpClient) : base(httpClient, "members") { }
}
