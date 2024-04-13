using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class StartProgram
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public FormStartProgramModel Model { get; private set; }

		public Command(long memberId, FormStartProgramModel model)
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
			long programId = request.Model.Sid.DecodeLong();

			var program = await db.Programs
				.Where(x => x.Id == programId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (program == null)
				return new CommandResult(false, "Invalid program.");

			program.DateStarted = request.Model.DateStart.Date;
			program.DateLastWorkoutUtc = null;
			program.NextTemplateIndex = request.Model.StartingTemplateIndex;
			
			await db.SaveChangesAsync(cancellationToken);
			return new CommandResult(true, "The program was started.");
		}
	}
}