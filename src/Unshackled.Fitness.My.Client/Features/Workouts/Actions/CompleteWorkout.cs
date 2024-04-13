using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class CompleteWorkout
{
	public class Command : IRequest<CommandResult>
	{
		public string WorkoutSid { get; private set; }

		public Command(string workoutSid)
		{
			WorkoutSid = workoutSid;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}finish", request.WorkoutSid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
