using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class UpdateTemplateSets
{
	public class Command : IRequest<CommandResult>
	{
		public string TemplateSid { get; private set; }
		public UpdateTemplateSetsModel Model { get; private set; }

		public Command(string templateSid, UpdateTemplateSetsModel model)
		{
			TemplateSid = templateSid;
			Model = model;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}update/{request.TemplateSid}/sets", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
