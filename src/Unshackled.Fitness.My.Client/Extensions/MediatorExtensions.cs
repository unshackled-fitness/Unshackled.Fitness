using MediatR;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Members.Actions;

namespace Unshackled.Fitness.My.Client.Extensions;

public static class MediatorExtensions
{
	public static async Task GetActiveMember(this IMediator mediator)
	{
		await mediator.Send(new GetActiveMember.Query());
	}

	public static async Task<CommandResult> SaveSettings(this IMediator mediator, AppSettings settings)
	{
		return await mediator.Send(new SaveSettings.Command(settings));
	}

	public static async Task<CommandResult> SetTheme(this IMediator mediator, Themes theme)
	{
		return await mediator.Send(new SetTheme.Command(theme));
	}
}
