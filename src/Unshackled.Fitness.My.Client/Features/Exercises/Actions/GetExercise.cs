using MediatR;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class GetExercise
{
	public class Query : IRequest<ExerciseModel>
	{
		public string ExerciseId { get; private set; }

		public Query(string exerciseId)
		{
			ExerciseId = exerciseId;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Query, ExerciseModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<ExerciseModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<ExerciseModel>($"{baseUrl}get/{request.ExerciseId}") ?? new();
		}
	}
}
