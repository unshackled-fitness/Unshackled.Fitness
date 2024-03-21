using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class DeleteSet
{
	public class Command : IRequest<CommandResult>
	{
		public string SetSid { get; private set; }

		public Command(string setSid)
		{
			SetSid = setSid;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}delete-set", request.SetSid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
