using System.Security.Claims;

namespace Unshackled.Fitness.My.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static string GetEmailClaim(this ClaimsPrincipal user)
	{
		return user.Identity?.Name ?? string.Empty;
	}
}
