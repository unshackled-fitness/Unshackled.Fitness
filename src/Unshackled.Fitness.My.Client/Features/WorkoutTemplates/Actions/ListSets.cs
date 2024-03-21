using MediatR;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class ListSets
{
	public class Query : IRequest<List<TemplateSetModel>>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Query, List<TemplateSetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<TemplateSetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<TemplateSetModel>>($"{baseUrl}get/{request.Sid}/sets") ??
				new List<TemplateSetModel>();
		}
	}
}
