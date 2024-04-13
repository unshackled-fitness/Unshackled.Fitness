using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Dashboard.Actions;

public class SaveMetric
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public SaveMetricModel Model { get; private set; }

		public Command(long memberId, SaveMetricModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long defId = request.Model.DefinitionSid.DecodeLong();

			if (defId == 0)
				return new CommandResult(false, "Invalid metric ID.");

			var definition = await db.MetricDefinitions
				.Where(x => x.MemberId == request.MemberId && x.Id == defId)
				.SingleOrDefaultAsync();

			if (definition == null) 
				return new CommandResult(false, "Invalid metric.");

			DateTime dateRecorded = request.Model.RecordedDate.ToDateTime(new TimeOnly(0, 0, 0), DateTimeKind.Local);

			var metric = await db.Metrics
				.Where(x => x.MemberId == request.MemberId 
					&& x.MetricDefinitionId == defId
					&& x.DateRecorded == dateRecorded)
				.SingleOrDefaultAsync();

			// Doesn't exist and has value, so add new
			if(metric == null && request.Model.RecordedValue > 0)
			{
				metric = new()
				{
					DateRecorded = dateRecorded,
					MemberId = request.MemberId,
					MetricDefinitionId = defId,
					RecordedValue = request.Model.RecordedValue
				};
				db.Metrics.Add(metric);
				await db.SaveChangesAsync();

				return new CommandResult(true, "Metric recorded.");
			}
			// exists and updating
			else if (metric != null && request.Model.RecordedValue > 0)
			{
				metric.RecordedValue = request.Model.RecordedValue;
				await db.SaveChangesAsync();

				return new CommandResult(true, "Metric updated.");
			}
			// exists and clearing
			else if (metric != null && request.Model.RecordedValue == 0)
			{
				db.Metrics.Remove(metric);
				await db.SaveChangesAsync();

				return new CommandResult(true, "Metric cleared.");
			}
			// doesn't exist, no value
			else
			{
				return new CommandResult(false, "Nothing to record.");
			}
		}
	}
}