using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class DeleteProgram
{
	public class Command : IRequest<CommandResult>
	{
		public string Sid { get; private set; }

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
			return await PostToCommandResultAsync($"{baseUrl}delete", request.Sid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
