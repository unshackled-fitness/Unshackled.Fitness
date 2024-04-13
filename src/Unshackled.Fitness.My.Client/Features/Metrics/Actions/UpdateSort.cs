using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class UpdateSort
{
	public class Command : IRequest<CommandResult>
	{
		public UpdateSortModel Model { get; private set; }

		public Command(UpdateSortModel model)
		{
			Model = model;
		}
	}

	public class Handler : BaseMetricHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}update-sort", request.Model)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
