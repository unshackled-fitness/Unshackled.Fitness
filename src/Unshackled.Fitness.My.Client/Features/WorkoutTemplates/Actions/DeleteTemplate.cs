using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class DeleteTemplate
{
	public class Command : IRequest<CommandResult>
	{
		public string TemplateSid { get; private set; }

		public Command(string templateSid)
		{
			TemplateSid= templateSid;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}delete", request.TemplateSid)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
