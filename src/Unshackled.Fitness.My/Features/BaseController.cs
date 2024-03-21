using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.My.Middleware;

namespace Unshackled.Fitness.My.Features;

[Authorize]
public abstract class BaseController : ControllerBase
{
	private IMediator? mediator;
	private SiteConfiguration? siteConfiguration;

	protected IMediator Mediator => mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
	protected SiteConfiguration SiteConfig => siteConfiguration ??= HttpContext.RequestServices.GetRequiredService<SiteConfiguration>();

	public ServerMember Member => HttpContext.Items.ContainsKey(ServerGlobals.MiddlewareItemKeys.Member)
		? (ServerMember)HttpContext.Items[ServerGlobals.MiddlewareItemKeys.Member]!
		: new();
}
