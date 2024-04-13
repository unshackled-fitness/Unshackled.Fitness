using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Actions;

public class AddTemplate
{
	public class Command : IRequest<CommandResult<string>>
	{
		public FormAddTemplateModel Model { get; private set; }

		public Command(FormAddTemplateModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseWorkoutHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormAddTemplateModel, string>($"{baseUrl}add-template", request.Model)
				?? new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
