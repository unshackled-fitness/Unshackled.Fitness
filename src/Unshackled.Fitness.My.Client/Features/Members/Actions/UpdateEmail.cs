using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class UpdateEmail
{
	public class Command : IRequest<CommandResult>
	{
		public FormChangeEmailModel Model { get; private set; }

		public Command(FormChangeEmailModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}update-email", request.Model) 
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
