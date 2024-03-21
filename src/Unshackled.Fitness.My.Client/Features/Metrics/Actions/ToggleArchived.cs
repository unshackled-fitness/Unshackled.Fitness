using MediatR;
using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Actions;

public class ToggleArchived
{
	public class Command : IRequest<CommandResult>
	{
		public string Sid { get; private set; }
		public bool IsArchived { get; private set; }

		public Command(string sid, bool isArchived)
		{
			Sid = sid;
			IsArchived = isArchived;
		}
	}

	public class Handler : BaseMetricHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(HttpClient httpClient) : base(httpClient) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			return await PostToCommandResultAsync($"{baseUrl}toggle-archived/{request.Sid}", request.IsArchived)
				?? new CommandResult(false, Globals.UnexpectedError);
		}
	}
}
