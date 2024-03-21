using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Web;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class GetAuthenticatorModel
{
	public class Query : IRequest<AuthenticatorModel>
	{
		public ClaimsPrincipal User { get; private set; }

		public Query(ClaimsPrincipal user)
		{
			User = user;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, AuthenticatorModel>
	{
		private readonly UserManager<UserEntity> userManager;
		private readonly SiteConfiguration siteConfig;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager, SiteConfiguration siteConfig) : base(db, mapper) 
		{
			this.userManager = userManager;
			this.siteConfig = siteConfig;
		}

		public async Task<AuthenticatorModel> Handle(Query request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new();

			AuthenticatorModel model = new();

			// Load the authenticator key & QR code URI to display on the form
			var unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
			if (string.IsNullOrEmpty(unformattedKey))
			{
				await userManager.ResetAuthenticatorKeyAsync(user);
				unformattedKey = await userManager.GetAuthenticatorKeyAsync(user);
			}

			model.SharedKey = FormatKey(unformattedKey!);

			var email = await userManager.GetEmailAsync(user);
			model.AuthenticatorUri = string.Format(
				CultureInfo.InvariantCulture,
				AuthenticatorModel.AuthenticatorUriFormat,
				HttpUtility.UrlPathEncode(siteConfig.SiteName ?? "Unshackled Fitness"),
				HttpUtility.UrlPathEncode(email),
				unformattedKey);

			return model;
		}

		private string FormatKey(string unformattedKey)
		{
			var result = new StringBuilder();
			int currentPosition = 0;
			while (currentPosition + 4 < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
				currentPosition += 4;
			}
			if (currentPosition < unformattedKey.Length)
			{
				result.Append(unformattedKey.AsSpan(currentPosition));
			}

			return result.ToString().ToLowerInvariant();
		}
	}
}
