using MediatR;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class GetWorkout
{
	public class Query : IRequest<FormWorkoutModel>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Query, FormWorkoutModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<FormWorkoutModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<FormWorkoutModel>($"{baseUrl}get/{request.Sid}") ??
				new FormWorkoutModel();
		}
	}
}
