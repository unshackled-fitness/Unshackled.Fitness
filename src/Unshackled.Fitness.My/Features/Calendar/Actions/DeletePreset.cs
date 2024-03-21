using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Calendar.Actions;

public class DeletePreset
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
			long id = request.Sid.DecodeLong();

			var preset = await db.MetricPresets
				.Where(x => x.MemberId == request.MemberId && x.Id == id)
				.SingleOrDefaultAsync();

			if (preset == null) 
				return new CommandResult(false, "Invalid preset.");

			db.MetricPresets.Remove(preset);
			await db.SaveChangesAsync();

			return new CommandResult(true, "Preset removed.");
		}
	}
}