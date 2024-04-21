using MediatR;
using Unshackled.Fitness.Core.Models.Calendars;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class GetCalendar
{
	public class Query : IRequest<CalendarModel> 
	{
		public string Sid { get; private set; }
		public SearchCalendarModel Model { get; private set; }

		public Query(string sid, SearchCalendarModel model)
		{
			Sid = sid;
			Model = model;
		}
	}

	public class Handler : BaseMetricHandler, IRequestHandler<Query, CalendarModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CalendarModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchCalendarModel, CalendarModel>($"{baseUrl}get-calendar/{request.Sid}", request.Model)
				?? new();
		}
	}
}
