using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class Disable2fa
{
	public class Command : IRequest<CommandResult> { }

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}disable-2fa", string.Empty) 
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
