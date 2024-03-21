using MediatR;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class GetCurrent2faStatus
{
	public class Query : IRequest<Current2faStatusModel> { }

	public class Handler : BaseMemberHandler, IRequestHandler<Query, Current2faStatusModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<Current2faStatusModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<Current2faStatusModel>($"{baseUrl}get-current-2fa-status") ?? new();
		}
	}
}
