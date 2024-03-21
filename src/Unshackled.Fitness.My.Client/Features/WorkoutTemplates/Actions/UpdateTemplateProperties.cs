using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class UpdateTemplateProperties
{
	public class Command : IRequest<CommandResult<TemplateModel>>
	{
		public FormTemplateModel Model { get; private set; }

		public Command(FormTemplateModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Command, CommandResult<TemplateModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<TemplateModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormTemplateModel, TemplateModel>($"{baseUrl}update", request.Model)
				?? new CommandResult<TemplateModel>(false, Globals.UnexpectedError);
		}
	}
}
