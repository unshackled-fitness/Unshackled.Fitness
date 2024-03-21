using MediatR;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class ListMergeExercises
{
	public class Query : IRequest<List<MergeExerciseModel>>
	{
		public List<string> Sids { get; private set; }

		public Query(List<string> uids)
		{
			Sids = uids;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Query, List<MergeExerciseModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<MergeExerciseModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<List<string>, List<MergeExerciseModel>>($"{baseUrl}merge/list", request.Sids) ??
				new();
		}
	}
}
