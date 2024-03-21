using MediatR;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class RestoreExercise
{
	public class Command : IRequest<ExerciseModel?>
	{
		public Guid MatchId { get; private set; }

		public Command(Guid matchId)
		{
			MatchId = matchId;
		}
	}

	public class Handler : BaseApiHandler, IRequestHandler<Command, ExerciseModel?>
	{
		public Handler(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "libraries") { }

		public async Task<ExerciseModel?> Handle(Command request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<ExerciseModel>($"{baseUrl}get-exercise/{request.MatchId}");
		}
	}
}
