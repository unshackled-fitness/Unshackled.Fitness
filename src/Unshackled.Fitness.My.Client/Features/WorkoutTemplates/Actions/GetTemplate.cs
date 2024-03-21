using MediatR;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class GetTemplate
{
	public class Query : IRequest<TemplateModel>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Query, TemplateModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<TemplateModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<TemplateModel>($"{baseUrl}get/{request.Sid}") ??
				new TemplateModel();
		}
	}
}
