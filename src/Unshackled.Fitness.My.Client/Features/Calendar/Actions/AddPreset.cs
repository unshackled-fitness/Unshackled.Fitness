using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Client.Features.Calendar.Actions;

public class AddPreset
{
	public class Command : IRequest<CommandResult<PresetModel>> 
	{
		public FormPresetModel Model { get; private set; }

		public Command(FormPresetModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseCalendarHandler, IRequestHandler<Command, CommandResult<PresetModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<PresetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormPresetModel, PresetModel>($"{baseUrl}add-preset", request.Model)
				?? new CommandResult<PresetModel>(false, Globals.UnexpectedError);
		}
	}
}
