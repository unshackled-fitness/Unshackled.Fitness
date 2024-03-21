using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Extensions;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class ListSetGroups
{
	public class Query : IRequest<List<TemplateSetGroupModel>>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }

		public Query(long memberId, long templateId)
		{
			MemberId = memberId;
			TemplateId = templateId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<TemplateSetGroupModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<TemplateSetGroupModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<TemplateSetGroupModel>(db.WorkoutTemplateSetGroups
				.AsNoTracking()
				.Include(x => x.WorkoutTemplate)
				.Where(x => x.WorkoutTemplateId == request.TemplateId && x.WorkoutTemplate.MemberId == request.MemberId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync() ?? new();
		}
	}
}
