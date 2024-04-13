using MediatR;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class SaveSettings
{
	public class Command : IRequest<CommandResult>
	{
		public AppSettings Settings { get; private set; }

		public Command(AppSettings settings)
		{
			Settings = settings;
		}
	}

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult>
	{
		private readonly AppState state = default!;

		public Handler(HttpClient httpClient, AppState stateContainer) : base(httpClient)
		{
			this.state = stateContainer;
		}

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var result = await PostToCommandResultAsync<AppSettings, Member>($"{baseUrl}save-settings", request.Settings)
				?? new CommandResult<Member>(false, "Could not update your settings");

			if (result.Success)
			{
				if (result.Payload != null)
				{
					state.SetActiveMember(result.Payload);
					return new CommandResult(true, result.Message);
				}
				else
				{
					return new CommandResult(false, "Member missing.");
				}
			}

			return new CommandResult(result.Success, result.Message);
		}
	}
}
