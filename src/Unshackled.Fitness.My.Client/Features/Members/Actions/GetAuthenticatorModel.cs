using MediatR;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class GetAuthenticatorModel
{
	public class Query : IRequest<AuthenticatorModel> { }

	public class Handler : BaseMemberHandler, IRequestHandler<Query, AuthenticatorModel>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<AuthenticatorModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await GetResultAsync<AuthenticatorModel>($"{baseUrl}get-authenticator-model") ?? new();
		}
	}
}
