using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Extensions;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class SearchTemplates
{
	public class Query : IRequest<SearchResult<TemplateListItem>>
	{
		public long MemberId { get; private set; }
		public SearchTemplateModel Model { get; private set; }

		public Query(long memberId, SearchTemplateModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<TemplateListItem>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<TemplateListItem>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<TemplateListItem>();
			var query = db.WorkoutTemplates
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId);

			if (request.Model.MuscleType != MuscleTypes.Any)
			{
				query = query.Where(x => x.MusclesTargeted!.Contains(request.Model.MuscleType.Title()));
			}

			if (!string.IsNullOrEmpty(request.Model.Title))
			{
				query = query.Where(x => x.Title.StartsWith(request.Model.Title));
			}

			result.Total = await query.CountAsync(cancellationToken);

			if (request.Model.Sorts.Any())
			{
				query = query.AddSorts(request.Model.Sorts);
			}
			else
			{
				query = query.OrderBy(x => x.Title);
			}

			query = query
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize);

			result.Data = await mapper.ProjectTo<TemplateListItem>(query)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
