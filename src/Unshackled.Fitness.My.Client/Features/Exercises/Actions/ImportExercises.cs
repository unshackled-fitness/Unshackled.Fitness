using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class ImportExercises
{
	public class Command : IRequest<CommandResult>
	{
		public List<LibraryListModel> SelectedExercises { get; private set; }

		public Command(List<LibraryListModel> selectedExercises)
		{
			SelectedExercises = selectedExercises;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			if (request.SelectedExercises.Count > AppGlobals.MaxSelectionLimit)
				return new CommandResult(false, "Maximum selection size exceeded.");

			return await PostToCommandResultAsync($"{baseUrl}import", request.SelectedExercises) ??
				new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
