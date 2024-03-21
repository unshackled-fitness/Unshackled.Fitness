using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class ListSets
{
	public class Query : IRequest<List<TemplateSetModel>>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }

		public Query(long memberId, long templateId)
		{
			MemberId = memberId;
			TemplateId = templateId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<TemplateSetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<TemplateSetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<TemplateSetModel>(db.WorkoutTemplateSets
				.Include(x => x.Exercise)
				.Include(x => x.WorkoutTemplate)
				.AsNoTracking()
				.Where(x => x.WorkoutTemplateId == request.TemplateId && x.WorkoutTemplate.MemberId == request.MemberId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();
		}
	}
}
