using MediatR;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class GetActiveMember
{
	public class Query : IRequest<Unit> { }

	public class Handler : BaseMemberHandler, IRequestHandler<Query, Unit>
	{
		AppState state = default!;

		public Handler(HttpClient httpClient, AppState stateContainer) : base(httpClient)
		{
			this.state = stateContainer;
		}

		public async Task<Unit> Handle(Query request, CancellationToken cancellationToken)
		{
			var member = await GetResultAsync<Member>($"{baseUrl}active");
			if (member != null)
			{
				state.SetActiveMember(member);
			}
			else
			{
				state.SetMemberLoaded(true);
			}
			return Unit.Value;
		}
	}
}
