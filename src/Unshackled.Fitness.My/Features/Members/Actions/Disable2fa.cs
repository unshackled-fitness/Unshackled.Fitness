using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class Disable2fa
{
	public class Command : IRequest<CommandResult>
	{
		public ClaimsPrincipal User { get; private set; }

		public Command(ClaimsPrincipal user)
		{
			User = user;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly UserManager<UserEntity> userManager;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager) : base(db, mapper)
		{
			this.userManager = userManager;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new CommandResult(false, "Invalid user.");

			if (!await userManager.GetTwoFactorEnabledAsync(user))
				return new CommandResult(false, "Cannot disable 2FA as it's not currently enabled.");

			var disable2faResult = await userManager.SetTwoFactorEnabledAsync(user, false);
			if (!disable2faResult.Succeeded)
				return new CommandResult(false, "Unexpected error occurred disabling 2FA.");

			return new CommandResult(true, "2fa has been disabled. You can reenable 2fa when you setup an authenticator app.");
		}
	}
}
