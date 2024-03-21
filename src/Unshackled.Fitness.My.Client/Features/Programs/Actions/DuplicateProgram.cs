using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Actions;

public class DuplicateProgram
{
	public class Command : IRequest<CommandResult<string>>
	{
		public FormUpdateProgramModel Model { get; private set; }

		public Command(FormUpdateProgramModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseProgramHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<FormUpdateProgramModel, string>($"{baseUrl}duplicate", request.Model)
				?? new CommandResult<string>(false, Globals.UnexpectedError);
		}
	}
}
