using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Client.Features.Calendar.Actions;

public class GetCalendar
{
	public class Query : IRequest<CalendarModel> 
	{
		public SearchCalendarModel Model { get; private set; }

		public Query(SearchCalendarModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseCalendarHandler, IRequestHandler<Query, CalendarModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CalendarModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<SearchCalendarModel, CalendarModel>($"{baseUrl}get", request.Model)
				?? new();
		}
	}
}
