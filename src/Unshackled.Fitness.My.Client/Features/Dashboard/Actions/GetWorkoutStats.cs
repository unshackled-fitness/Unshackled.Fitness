using MediatR;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class GetWorkoutStats
{
	public class Query : IRequest<WorkoutStatsModel>
	{
		public DateTime FromDate { get; private set; }

		public Query(DateTime fromDate)
		{
			FromDate = fromDate;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Query, WorkoutStatsModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<WorkoutStatsModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<DateTime, WorkoutStatsModel>($"{baseUrl}workout-stats", request.FromDate) ??
				new WorkoutStatsModel();
		}
	}
}
