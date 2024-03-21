using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class GetCurrent2faStatus
{
	public class Query : IRequest<Current2faStatusModel>
	{
		public ClaimsPrincipal User { get; private set; }
		public ITrackingConsentFeature? TrackingConsent {  get; private set; }

		public Query(ClaimsPrincipal user, ITrackingConsentFeature? trackingConsent)
		{
			User = user;
			TrackingConsent = trackingConsent;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, Current2faStatusModel>
	{
		private readonly UserManager<UserEntity> userManager;
		private readonly SignInManager<UserEntity> signInManager;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : base(db, mapper) 
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<Current2faStatusModel> Handle(Query request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new();

			Current2faStatusModel status = new();

			status.CanTrack = true; // request.TrackingConsent?.CanTrack ?? true;
			status.HasAuthenticator = await userManager.GetAuthenticatorKeyAsync(user) is not null;
			status.Is2faEnabled = await userManager.GetTwoFactorEnabledAsync(user);
			status.IsMachineRemembered = await signInManager.IsTwoFactorClientRememberedAsync(user);
			status.RecoveryCodesLeft = await userManager.CountRecoveryCodesAsync(user);

			return status;
		}
	}
}
