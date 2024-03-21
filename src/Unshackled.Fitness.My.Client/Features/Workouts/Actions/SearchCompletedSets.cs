using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class SearchCompletedSets
{
	public class Query : IRequest<SearchResult<CompletedSetModel>>
	{
		public SearchSetModel Model { get; private set; }

		public Query(SearchSetModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Query, SearchResult<CompletedSetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<SearchResult<CompletedSetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchSetModel, SearchResult<CompletedSetModel>>($"{baseUrl}search-completed-sets", request.Model) ??
				new SearchResult<CompletedSetModel>();
		}
	}
}
