using MediatR;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class ListProgramItems
{
	public class Query : IRequest<List<ProgramListModel>>
	{
		public DateTime DisplayDate { get; private set; }

		public Query(DateTime displayDate)
		{
			DisplayDate = displayDate;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Query, List<ProgramListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<ProgramListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await PostToResultAsync<DateTime, List<ProgramListModel>>($"{baseUrl}list-program-items", request.DisplayDate) ?? new();
		}
	}
}
