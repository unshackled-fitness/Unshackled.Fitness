using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class ListTasks
{
	public class Query : IRequest<List<TemplateTaskModel>>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }
		public WorkoutTaskTypes TaskType { get; private set; }

		public Query(long memberId, long templateId, WorkoutTaskTypes taskType)
		{
			MemberId = memberId;
			TemplateId = templateId;
			TaskType = taskType;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<TemplateTaskModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<TemplateTaskModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<TemplateTaskModel>(db.WorkoutTemplateTasks
				.AsNoTracking()
				.Include(x => x.WorkoutTemplate)
				.Where(x => x.WorkoutTemplateId == request.TemplateId && x.WorkoutTemplate.MemberId == request.MemberId && x.Type == request.TaskType)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();
		}
	}
}
