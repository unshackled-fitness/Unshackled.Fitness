using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class ToggleArchived
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public long DefinitionId { get; private set; }
		public bool IsArchived { get; private set; }

		public Command(long memberId, long definitionId, bool isArchived)
		{
			MemberId = memberId;
			DefinitionId = definitionId;
			IsArchived = isArchived;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var definition = await db.MetricDefinitions
				.Where(x => x.Id == request.DefinitionId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (definition == null)
				return new CommandResult(false, "Invalid metric.");

			definition.IsArchived = request.IsArchived;			
			await db.SaveChangesAsync(cancellationToken);
			return new CommandResult(true, request.IsArchived ? "Metric archived." : "Metric restored.");
		}
	}
}