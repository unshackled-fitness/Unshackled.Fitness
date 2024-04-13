using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class AddAuthenticator
{
	public class Command : IRequest<CommandResult<IEnumerable<string>?>>
	{
		public ClaimsPrincipal User { get; private set; }
		public FormAuthenticatorModel Model { get; private set; }

		public Command(ClaimsPrincipal user, FormAuthenticatorModel model)
		{
			User = user;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<IEnumerable<string>?>>
	{
		private readonly UserManager<UserEntity> userManager;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager) : base(db, mapper)
		{
			this.userManager = userManager;
		}

		public async Task<CommandResult<IEnumerable<string>?>> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new CommandResult<IEnumerable<string>?>(false, "Invalid user.");

			// Strip spaces and hyphens
			var verificationCode = request.Model.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

			var is2faTokenValid = await userManager.VerifyTwoFactorTokenAsync(
				user, userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

			if (!is2faTokenValid)
			{
				return new CommandResult<IEnumerable<string>?>(false, "Verification code is invalid.");
			}

			var result = await userManager.SetTwoFactorEnabledAsync(user, true);

			IEnumerable<string>? recoveryCodes = null;
			if (result.Succeeded)
			{
				recoveryCodes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
			}

			return new CommandResult<IEnumerable<string>?>(true, "Your authenticator app has been verified.", recoveryCodes);
		}
	}
}
