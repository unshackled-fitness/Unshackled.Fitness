using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class DeleteProgram
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string ProgramSid { get; private set; }

		public Command(long memberId, string programSid)
		{
			MemberId = memberId;
			ProgramSid = programSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long programId = request.ProgramSid.DecodeLong();

			if (programId == 0)
				return new CommandResult<string>(false, "Invalid program ID.");

			var program = await db.Programs
				.Where(x => x.Id == programId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (program == null)
				return new CommandResult(false, "Invalid program.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				await db.ProgramTemplates
					.Where(x => x.ProgramId == program.Id)
					.DeleteFromQueryAsync(cancellationToken);

				db.Programs.Remove(program);
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Program deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "Database error. Program could not be deleted.");
			}
		}
	}
}