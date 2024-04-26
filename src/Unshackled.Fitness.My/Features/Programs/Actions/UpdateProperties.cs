using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class UpdateProperties
{
	public class Command : IRequest<CommandResult<ProgramModel>>
	{
		public long MemberId { get; private set; }
		public FormUpdateProgramModel Model { get; private set; }

		public Command(long memberId, FormUpdateProgramModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<ProgramModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<ProgramModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			long programId = request.Model.Sid.DecodeLong();

			var program = await db.Programs
				.Where(x => x.Id == programId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (program == null)
				return new CommandResult<ProgramModel>(false, "Invalid program.");

			// Update program
			program.Description = request.Model.Description?.Trim();
			program.Title = request.Model.Title.Trim();

			// Mark modified to avoid missing string case changes.
			db.Entry(program).Property(x => x.Title).IsModified = true;
			db.Entry(program).Property(x => x.Description).IsModified = true;

			await db.SaveChangesAsync(cancellationToken);

			var p = mapper.Map<ProgramModel>(program);

			p.Templates = await mapper.ProjectTo<ProgramTemplateModel>(db.ProgramTemplates
					.AsNoTracking()
					.Include(x => x.WorkoutTemplate)
					.Where(x => x.ProgramId == program.Id)
					.OrderBy(x => x.SortOrder))
					.ToListAsync();

			return new CommandResult<ProgramModel>(true, "Program updated.", p);
		}
	}
}