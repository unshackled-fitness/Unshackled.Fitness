using MediatR;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class ListTemplates
{
	public class Query : IRequest<List<TemplateListModel>> { }

	public class Handler : BaseProgramHandler, IRequestHandler<Query, List<TemplateListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<TemplateListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<TemplateListModel>>($"{baseUrl}list-templates") ?? new ();
		}
	}
}
