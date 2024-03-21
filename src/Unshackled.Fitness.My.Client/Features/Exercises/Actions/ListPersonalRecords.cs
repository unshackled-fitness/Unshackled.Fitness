using MediatR;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Actions;

public class ListPersonalRecords
{
	public class Query : IRequest<List<RecordListModel>>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseExerciseHandler, IRequestHandler<Query, List<RecordListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<List<RecordListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<List<RecordListModel>>($"{baseUrl}list/{request.Sid}/records") ??
				new();
		}
	}
}
