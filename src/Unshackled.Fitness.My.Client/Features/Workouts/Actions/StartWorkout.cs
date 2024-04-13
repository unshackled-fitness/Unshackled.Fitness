using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class StartWorkout
{
	public class Command : IRequest<CommandResult<DateTime>>
	{
		public string WorkoutSid { get; private set; }

		public Command(string workoutSid)
		{
			WorkoutSid = workoutSid;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult<DateTime>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<DateTime>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<string, DateTime>($"{baseUrl}start", request.WorkoutSid)
				?? new CommandResult<DateTime>(false, Globals.UnexpectedError);
		}
	}
}
