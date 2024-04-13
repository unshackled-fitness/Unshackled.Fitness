using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class UpdatePassword
{
	public class Command : IRequest<CommandResult>
	{
		public ClaimsPrincipal User { get; private set; }
		public FormChangePasswordModel Model { get; private set; }

		public Command(ClaimsPrincipal user, FormChangePasswordModel model)
		{
			User = user;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly UserManager<UserEntity> userManager; 
		private readonly SignInManager<UserEntity> signInManager;

		public Handler(BaseDbContext db, IMapper mapper, UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager) : base(db, mapper)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var user = await userManager.GetUserAsync(request.User);

			if (user == null)
				return new CommandResult(false, "Invalid user.");

			var changePasswordResult = await userManager.ChangePasswordAsync(user, request.Model.OldPassword, request.Model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				return new CommandResult(false, $"Error: {string.Join(",", changePasswordResult.Errors.Select(error => error.Description))}");
			}

			await signInManager.RefreshSignInAsync(user);

			return new CommandResult(true, "Your password has been changed.");
		}
	}
}
