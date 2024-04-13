using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class UpdateSetProperties
{
	public class Command : IRequest<CommandResult>
	{
		public FormWorkoutSetModel Set { get; private set; }

		public Command(FormWorkoutSetModel set)
		{
			Set = set;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}update-set-properties", request.Set)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
