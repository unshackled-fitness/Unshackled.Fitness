using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class ResendVerificationEmail
{
	public class Command : IRequest<CommandResult>
	{
		public ClaimsPrincipal User { get; private set; }
		public string ConfirmUrl { get; private set; }

		public Command(ClaimsPrincipal user, string confirmUrl)
		{
			User = user;
			ConfirmUrl = confirmUrl;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly UserManager<UserEntity> userManager; 
		private readonly SignInManager<UserEntity> signInManager;
		private readonly IEmailSender<UserEntity> emailSender;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager, 
			SignInManager<UserEntity> signInManager, IEmailSender<UserEntity> emailSender) : base(db, mapper)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.emailSender = emailSender;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new CommandResult(false, "Invalid user.");

			if (string.IsNullOrEmpty(user.Email))
				return new CommandResult(false, "User does not have an email address.");

			if (string.IsNullOrEmpty(request.ConfirmUrl))
				return new CommandResult(false, "Invalid Confirmation Url");

			string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			string callbackUrl = $"{request.ConfirmUrl}?userId={user.Id}&code={code}";

			await emailSender.SendConfirmationLinkAsync(user, user.Email, HtmlEncoder.Default.Encode(callbackUrl));

			return new CommandResult(true, "Verification email sent. Please check your email.");
		}
	}
}
