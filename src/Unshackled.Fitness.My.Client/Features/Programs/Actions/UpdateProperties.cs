using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class UpdateProperties
{
	public class Command : IRequest<CommandResult<ProgramModel>>
	{
		public FormUpdateProgramModel Model { get; private set; }

		public Command(FormUpdateProgramModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseProgramHandler, IRequestHandler<Command, CommandResult<ProgramModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<ProgramModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormUpdateProgramModel, ProgramModel>($"{baseUrl}update", request.Model)
				?? new CommandResult<ProgramModel>(false, Globals.UnexpectedError);
		}
	}
}
