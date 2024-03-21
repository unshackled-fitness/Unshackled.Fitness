using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class GetTemplate
{
	public class Query : IRequest<TemplateModel>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }

		public Query(long memberId, long templateId)
		{
			MemberId = memberId;
			TemplateId = templateId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, TemplateModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<TemplateModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<TemplateModel>(db.WorkoutTemplates
				.AsNoTracking()
				.Where(x => x.Id == request.TemplateId && x.MemberId == request.MemberId))
				.SingleOrDefaultAsync() ?? new();
		}
	}
}
