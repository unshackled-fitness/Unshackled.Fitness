using MediatR;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class ListMetrics
{
	public class Query : IRequest<MetricGridModel>
	{
		public DateTime DisplayDate { get; private set; }

		public Query(DateTime displayDate)
		{
			DisplayDate = displayDate;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Query, MetricGridModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<MetricGridModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<DateTime, MetricGridModel>($"{baseUrl}list-metrics", request.DisplayDate) ??
				new MetricGridModel();
		}
	}
}
