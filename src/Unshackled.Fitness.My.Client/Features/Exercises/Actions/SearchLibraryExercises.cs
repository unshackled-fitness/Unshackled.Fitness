using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class SearchLibraryExercises
{
	public class Query : IRequest<SearchResult<LibraryListModel>>
	{
		public SearchLibraryModel Model { get; private set; }

		public Query(SearchLibraryModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseApiHandler, IRequestHandler<Query, SearchResult<LibraryListModel>>
	{
		public Handler(IHttpClientFactory httpClientFactory) : base(httpClientFactory, "libraries") { }

		public async Task<SearchResult<LibraryListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchLibraryModel, SearchResult<LibraryListModel>>($"{baseUrl}search-exercises", request.Model) ??
				new SearchResult<LibraryListModel>();
		}
	}
}
