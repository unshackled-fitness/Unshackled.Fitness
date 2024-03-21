using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class SearchTemplates
{
	public class Query : IRequest<SearchResult<TemplateListItem>>
	{
		public SearchTemplateModel Model { get; private set; }

		public Query(SearchTemplateModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Query, SearchResult<TemplateListItem>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<SearchResult<TemplateListItem>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchTemplateModel, SearchResult<TemplateListItem>>($"{baseUrl}search", request.Model) ??
				new SearchResult<TemplateListItem>();
		}
	}
}
