using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Client.Features.Dashboard.Actions;

public class SaveMetric
{
	public class Command : IRequest<CommandResult>
	{
		public SaveMetricModel Model { get; private set; }

		public Command(SaveMetricModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseDashboardHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}save-metric", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
