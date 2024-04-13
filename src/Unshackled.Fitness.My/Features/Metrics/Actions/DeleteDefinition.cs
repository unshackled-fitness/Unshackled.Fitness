using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class DeleteDefinition
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string Sid { get; private set; }

		public Command(long memberId, string sid)
		{
			MemberId = memberId;
			Sid = sid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long defId = request.Sid.DecodeLong();

			var definition = await db.MetricDefinitions
				.Where(x => x.Id == defId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (definition == null)
				return new CommandResult(false, "Invalid metric.");

			// Delete all related records
			await db.Metrics
				.Where(x => x.MetricDefinitionId == definition.Id)
				.DeleteFromQueryAsync();

			db.MetricDefinitions.Remove(definition);			
			await db.SaveChangesAsync(cancellationToken);
			return new CommandResult(true, "Metric deleted.");
		}
	}
}