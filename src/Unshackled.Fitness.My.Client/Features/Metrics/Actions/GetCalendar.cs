using MediatR;
using Unshackled.Fitness.Core.Models.Calendars;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class GetCalendar
{
	public class Query : IRequest<CalendarModel> 
	{
		public string Sid { get; private set; }
		public DateOnly ToDate { get; private set; }

		public Query(string sid, DateOnly toDate)
		{
			Sid = sid;
			ToDate = toDate;
		}
	}

	public class Handler : BaseMetricHandler, IRequestHandler<Query, CalendarModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CalendarModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<DateOnly, CalendarModel>($"{baseUrl}get-calendar/{request.Sid}", request.ToDate)
				?? new();
		}
	}
}
