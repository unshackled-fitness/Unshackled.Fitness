using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Calendar.Actions;

public class DeletePreset
{
	public class Command : IRequest<CommandResult> 
	{
		public string PresetSid { get; private set; }

		public Command(string sid)
		{
			PresetSid = sid;
		}
	}

	public class Handler : BaseCalendarHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}delete-preset", request.PresetSid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
