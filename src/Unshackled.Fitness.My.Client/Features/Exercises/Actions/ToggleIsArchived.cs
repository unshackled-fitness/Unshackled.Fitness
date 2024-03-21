using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class ToggleIsArchived
{
	public class Command : IRequest<CommandResult<bool>>
	{
		public string Sid { get; private set; }

		public Command(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Command, CommandResult<bool>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<string, bool>($"{baseUrl}toggle/archived", request.Sid) ??
				new CommandResult<bool>(false, Globals.UnexpectedError);
		}
	}
}
