using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class StopProgram
{
	public class Command : IRequest<CommandResult>
	{
		public string Sid { get; set; }

		public Command(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseProgramHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}stop", request.Sid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
