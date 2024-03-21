using MediatR;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class ListMetrics
{
	public class Query : IRequest<MetricListModel> { }

	public class Handler : BaseMetricHandler, IRequestHandler<Query, MetricListModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<MetricListModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<MetricListModel>($"{baseUrl}list") ?? new();
		}
	}
}
