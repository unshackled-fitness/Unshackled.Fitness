using MediatR;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class ListSetGroups
{
	public class Query : IRequest<List<TemplateSetGroupModel>>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Query, List<TemplateSetGroupModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<TemplateSetGroupModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<TemplateSetGroupModel>>($"{baseUrl}get/{request.Sid}/groups") ??
				new List<TemplateSetGroupModel>();
		}
	}
}
