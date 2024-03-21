using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class SearchResults
{
	public class Query : IRequest<SearchResult<ResultListModel>>
	{
		public SearchResultsModel Model { get; private set; }

		public Query(SearchResultsModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Query, SearchResult<ResultListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<SearchResult<ResultListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchResultsModel, SearchResult<ResultListModel>>($"{baseUrl}search-results", request.Model) ??
				new SearchResult<ResultListModel>();
		}
	}
}
