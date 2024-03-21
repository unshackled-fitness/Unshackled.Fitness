using MediatR;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class GetProgram
{
	public class Query : IRequest<ProgramModel>
	{
		public string Sid { get; private set; }

		public Query(string sid)
		{
			Sid = sid;
		}
	}

	public class Handler : BaseProgramHandler, IRequestHandler<Query, ProgramModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<ProgramModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<ProgramModel>($"{baseUrl}get/{request.Sid}") ?? new();
		}
	}
}
