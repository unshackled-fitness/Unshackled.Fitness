namespace Unshackled.Fitness.My.Client.Features.Calendar.Actions;

public abstract class BaseCalendarHandler : BaseHandler
{
	public BaseCalendarHandler(HttpClient httpClient) : base(httpClient, "calendar") { }
}
