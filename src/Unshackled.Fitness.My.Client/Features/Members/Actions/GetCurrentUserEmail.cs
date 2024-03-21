using MediatR;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class GetCurrentUserEmail
{
	public class Query : IRequest<CurrentUserEmailModel> { }

	public class Handler : BaseMemberHandler, IRequestHandler<Query, CurrentUserEmailModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CurrentUserEmailModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<CurrentUserEmailModel>($"{baseUrl}get-current-user-email") ?? new();
		}
	}
}
