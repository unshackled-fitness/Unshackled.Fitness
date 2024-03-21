using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class UpdateExercise
{
	public class Command : IRequest<CommandResult<ExerciseModel>>
	{
		public FormExerciseModel Model { get; private set; }

		public Command(FormExerciseModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Command, CommandResult<ExerciseModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<ExerciseModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormExerciseModel, ExerciseModel>($"{baseUrl}update", request.Model) ??
				new CommandResult<ExerciseModel>(false, Globals.UnexpectedError);
		}
	}
}
