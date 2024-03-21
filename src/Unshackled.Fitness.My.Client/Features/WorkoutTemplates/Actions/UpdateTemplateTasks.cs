using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class UpdateTemplateTasks
{
	public class Command : IRequest<CommandResult>
	{
		public string TemplateSid { get; private set; }
		public UpdateTemplateTasksModel Model { get; private set; }

		public Command(string templateSid, UpdateTemplateTasksModel model)
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
			return await PostToCommandResultAsync($"{baseUrl}update/{request.TemplateSid}/tasks", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
