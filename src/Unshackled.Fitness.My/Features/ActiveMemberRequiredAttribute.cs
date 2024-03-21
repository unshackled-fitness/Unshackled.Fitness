using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Unshackled.Fitness.My.Middleware;

namespace Unshackled.Fitness.My.Features;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ActiveMemberRequiredAttribute : TypeFilterAttribute
{
	public ActiveMemberRequiredAttribute() : base(typeof(ActiveMemberRequiredFilter)) { }

	private class ActiveMemberRequiredFilter : IAsyncActionFilter
	{
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var member = context.HttpContext.Items[ServerGlobals.MiddlewareItemKeys.Member] as ServerMember;

			if (member == null || !member.IsActive)
			{
				context.Result = new NotFoundResult();
				return;
			}
			await next();
		}
	}
}