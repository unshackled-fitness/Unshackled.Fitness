using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Actions;

public class DuplicateTemplate
{
	public class Command : IRequest<CommandResult<string>>
	{
		public string Sid { get; private set; }
		public FormTemplateModel Model { get; private set; }

		public Command(string sid, FormTemplateModel model)
		{
			Sid = sid;
			Model = model;
		}
	}

	public class Handler : BaseTemplateHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormTemplateModel, string>($"{baseUrl}duplicate/{request.Sid}", request.Model)
				?? new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
