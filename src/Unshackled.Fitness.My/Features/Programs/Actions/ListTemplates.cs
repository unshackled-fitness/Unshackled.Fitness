using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Programs.Models;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class ListTemplates
{
	public class Query : IRequest<List<TemplateListModel>>
	{
		public long MemberId { get; private set; }

		public Query(long memberId)
		{
			MemberId = memberId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<TemplateListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<TemplateListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<TemplateListModel>(db.WorkoutTemplates
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.Title))
				.ToListAsync();
		}
	}
}
