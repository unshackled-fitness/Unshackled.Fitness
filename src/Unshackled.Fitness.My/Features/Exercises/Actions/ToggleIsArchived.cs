using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class ToggleIsArchived
{
	public class Command : IRequest<CommandResult<bool>>
	{
		public long MemberId { get; private set; }
		public string Sid { get; private set; }

		public Command(long memberId, string sid)
		{
			MemberId = memberId;
			Sid = sid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<bool>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<bool>> Handle(Command request, CancellationToken cancellationToken)
		{
			long exerciseId = request.Sid.DecodeLong();

			if (exerciseId == 0)
				return new CommandResult<bool>(false, "Invalid exercise.");

			var exercise = await db.Exercises
				.Where(e => e.Id == exerciseId && e.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (exercise is null)
				return new CommandResult<bool>(false, "Invalid exercise.");

			exercise.IsArchived = !exercise.IsArchived;
			await db.SaveChangesAsync(cancellationToken);

			string msg = "Exercise archived.";
			if (!exercise.IsArchived)
				msg = "Exercise restored.";

			return new CommandResult<bool>(true, msg, exercise.IsArchived);
		}
	}
}