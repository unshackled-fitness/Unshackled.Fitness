using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Dashboard.Actions;

public class SkipWorkout
{
	public class Command : IRequest<CommandResult<ProgramListModel>>
	{
		public long MemberId { get; private set; }
		public string ProgramSid { get; private set; }

		public Command(long memberId, string programSid)
		{
			MemberId = memberId;
			ProgramSid = programSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<ProgramListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<ProgramListModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			if (string.IsNullOrEmpty(request.ProgramSid))
				return new CommandResult<ProgramListModel>(false, "Program ID missing.");

			long programId = request.ProgramSid.DecodeLong();

			if (programId == 0)
				return new CommandResult<ProgramListModel>(false, "Invalid program ID.");

			var program = await db.Programs
				.Include(x => x.Templates.OrderBy(y => y.SortOrder))
				.Where(x => x.Id == programId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (program == null)
				return new CommandResult<ProgramListModel>(false, "Invalid program.");

			if (program.ProgramType != ProgramTypes.Sequential)
				return new CommandResult<ProgramListModel>(false, "Could not skip the workout.");

			// Get next sort order. Could be missing numbers in sequence so use >=.
			// Defaults to zero if nothing found.
			int nextIndex = program.Templates
				.Where(x => x.SortOrder >= program.NextTemplateIndex + 1)
				.Select(x => x.SortOrder)
				.FirstOrDefault();

			program.NextTemplateIndex = nextIndex;
			await db.SaveChangesAsync();

			var model = program.Templates
				.Where(x => x.SortOrder == nextIndex)
				.Select(x => new ProgramListModel
				{
					IsStarted = false,
					ProgramSid = program.Id.Encode(),
					ProgramTitle = program.Title,
					ProgramType = ProgramTypes.Sequential,
					Sid = x.WorkoutTemplateId.Encode(),
					Title = db.WorkoutTemplates
						.Where(y => y.Id == x.WorkoutTemplateId).Select(y => y.Title).SingleOrDefault() ?? string.Empty
				})
				.SingleOrDefault() ?? new();

			return new CommandResult<ProgramListModel>(true, "Workout skipped.", model);
		}
	}
}