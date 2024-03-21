using MediatR;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class GetDefinition
{
	public class Query : IRequest<FormMetricDefinitionModel> 
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseMetricHandler, IRequestHandler<Query, FormMetricDefinitionModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<FormMetricDefinitionModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<FormMetricDefinitionModel>($"{baseUrl}get/{request.Sid}") ?? new();
		}
	}
}
