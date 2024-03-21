using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Data.Entities;

namespace Unshackled.Fitness.My.Services;

internal sealed class IdentityUserAccessor(UserManager<UserEntity> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<UserEntity> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("account/invalid-user", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
