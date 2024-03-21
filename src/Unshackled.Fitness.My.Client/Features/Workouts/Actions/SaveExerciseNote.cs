using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class SaveExerciseNote
{
	public class Command : IRequest<CommandResult>
	{
		public ExerciseNoteModel Model { get; private set; }

		public Command(ExerciseNoteModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}save-note", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
