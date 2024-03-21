using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class GetProgram
{
	public class Query : IRequest<ProgramModel>
	{
		public long MemberId { get; private set; }
		public long Id { get; private set; }

		public Query(long memberId, long id)
		{
			MemberId = memberId;
			Id = id;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, ProgramModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<ProgramModel> Handle(Query request, CancellationToken cancellationToken)
		{
			var program = await mapper.ProjectTo<ProgramModel>(db.Programs
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId && x.Id == request.Id))
				.SingleOrDefaultAsync();

			if (program != null)
			{
				program.Templates = await mapper.ProjectTo<ProgramTemplateModel>(db.ProgramTemplates
					.AsNoTracking()
					.Include(x => x.WorkoutTemplate)
					.Where(x => x.ProgramId == request.Id)
					.OrderBy(x => x.SortOrder))
					.ToListAsync();
			}
			else
			{
				program = new();
			}

			return program;
		}
	}
}
