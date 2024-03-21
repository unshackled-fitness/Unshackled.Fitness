using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class AddAuthenticator
{
	public class Command : IRequest<CommandResult<IEnumerable<string>?>>
	{
		public FormAuthenticatorModel Model { get; private set; }

		public Command(FormAuthenticatorModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult<IEnumerable<string>?>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<IEnumerable<string>?>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormAuthenticatorModel, IEnumerable<string>?> ($"{baseUrl}add-authenticator", request.Model) 
				?? new CommandResult<IEnumerable<string>?>(false, Globals.UnexpectedError);
		}
	}
}
