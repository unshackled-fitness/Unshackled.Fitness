using MediatR;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class SearchPrograms
{
	public class Query : IRequest<SearchResult<ProgramListModel>>
	{
		public SearchProgramModel Model { get; private set; }

		public Query(SearchProgramModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseProgramHandler, IRequestHandler<Query, SearchResult<ProgramListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<SearchResult<ProgramListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchProgramModel, SearchResult<ProgramListModel>>($"{baseUrl}search", request.Model) ??
				new SearchResult<ProgramListModel>();
		}
	}
}
