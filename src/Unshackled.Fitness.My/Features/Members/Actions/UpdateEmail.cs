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
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class UpdateEmail
{
	public class Command : IRequest<CommandResult>
	{
		public ClaimsPrincipal User { get; private set; }
		public FormChangeEmailModel Model { get; private set; }

		public Command(ClaimsPrincipal user, FormChangeEmailModel model)
		{
			User = user;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly UserManager<UserEntity> userManager;
		private readonly IEmailSender<UserEntity> emailSender;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager, 
			IEmailSender<UserEntity> emailSender) : base(db, mapper)
		{
			this.userManager = userManager;
			this.emailSender = emailSender;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new CommandResult(false, "Invalid user.");

			if (string.IsNullOrEmpty(request.Model.BaseUrl))
				return new CommandResult(false, "Invalid Confirmation Url");

			if (request.Model.NewEmail is null || request.Model.NewEmail == user.Email)
				return new CommandResult(false, "Your email is unchanged.");

			string code = await userManager.GenerateChangeEmailTokenAsync(user, request.Model.NewEmail);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			string callbackUrl = $"{request.Model.BaseUrl}?userId={user.Id}&email={request.Model.NewEmail}&code={code}";

			await emailSender.SendConfirmationLinkAsync(user, request.Model.NewEmail, HtmlEncoder.Default.Encode(callbackUrl));

			return new CommandResult(true, "Confirmation link sent to new email address. Please check your email.");
		}
	}
}
