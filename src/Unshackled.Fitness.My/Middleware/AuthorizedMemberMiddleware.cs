using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Middleware;

public class AuthorizedMemberMiddleware
{
	private readonly RequestDelegate next;

	public AuthorizedMemberMiddleware(RequestDelegate next)
	{
		this.next = next;
	}

	public async Task InvokeAsync(HttpContext context, BaseDbContext db)
	{
		var path = context.Request.Path;
		// Skip paths not starting with /api
		if (!path.StartsWithSegments("/api"))
		{
			await next(context);
			return;
		}

		if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		string email = context.User.GetEmailClaim();
		if (string.IsNullOrEmpty(email))
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		// Member is not required for these paths because record may not have
		// been created yet or we are updating the users membership
		if (path.StartsWithSegments("/api/members/active") ||
			path.StartsWithSegments("/api/membership"))
		{
			await next(context);
			return;
		}

		var member = await db.Members
			.Where(x => x.Email == email)
			.SingleOrDefaultAsync();

		// Member doesn't exist
		if (member == null)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		context.Items[ServerGlobals.MiddlewareItemKeys.Member] = new ServerMember
		{
			DateCreatedUtc = member.DateCreatedUtc,
			DateLastModifiedUtc = member.DateLastModifiedUtc,
			Email = member.Email,
			Id = member.Id,
			Sid = member.Id.Encode(),
			IsActive = member.IsActive
		};

		await next(context);
	}
}
