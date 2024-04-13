using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Members.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class UpdatePassword
{
	public class Command : IRequest<CommandResult>
	{
		public FormChangePasswordModel Model { get; private set; }

		public Command(FormChangePasswordModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}update-password", request.Model) 
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
