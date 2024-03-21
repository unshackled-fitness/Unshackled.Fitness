using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class AddWorkout
{
	public class Command : IRequest<CommandResult<string>>
	{
		public string TemplateSid { get; private set; }

		public Command(string templateSid)
		{
			TemplateSid = templateSid;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<string, string>($"{baseUrl}add-workout", request.TemplateSid)
				?? new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
