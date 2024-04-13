using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class HideGettingStarted
{
	public class Command : IRequest<CommandResult> { }

	public class Handler : BaseDashboardHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly AppState state = default!;

		public Handler(HttpClient httpClient, AppState state) : base(httpClient)
		{
			this.state = state;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var result = await PostToCommandResultAsync<bool, Member>($"{baseUrl}hide-getting-started", true)
				?? new CommandResult<Member>(false, Globals.UnexpectedError);

			if (result.Success && result.Payload != null)
				state.SetActiveMember(result.Payload);

			return new CommandResult(result.Success, result.Message);
		}
	}
}
