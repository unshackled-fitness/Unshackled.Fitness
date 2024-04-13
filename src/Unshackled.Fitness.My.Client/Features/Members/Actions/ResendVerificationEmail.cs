using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Members.Actions;

public class ResendVerificationEmail
{
	public class Command : IRequest<CommandResult>
	{
		public string ConfirmUrl { get; private set; }

		public Command(string confirmUrl)
		{
			ConfirmUrl = confirmUrl;
		}
	}

	public class Handler : BaseMemberHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}resend-verification-email", request.ConfirmUrl) 
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
