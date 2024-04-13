using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class SkipWorkout
{
	public class Command : IRequest<CommandResult<ProgramListModel>>
	{
		public string ProgramSid { get; private set; }

		public Command(string programSid)
		{
			ProgramSid = programSid;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Command, CommandResult<ProgramListModel>>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult<ProgramListModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync<string, ProgramListModel>($"{baseUrl}skip-workout", request.ProgramSid)
				?? new CommandResult<ProgramListModel>(false, Globals.UnexpectedError);
		}
	}
}
