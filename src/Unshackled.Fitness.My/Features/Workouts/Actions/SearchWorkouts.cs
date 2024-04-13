using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Extensions;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class SearchWorkouts
{
	public class Query : IRequest<SearchResult<WorkoutListModel>>
	{
		public long MemberId { get; private set; }
		public SearchWorkoutModel Model { get; private set; }

		public Query(long memberId, SearchWorkoutModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<WorkoutListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<WorkoutListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<WorkoutListModel>();
			var query = db.Workouts
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId);

			if (request.Model.DateStart.HasValue)
			{
				query = query.Where(x => x.DateStartedUtc >= request.Model.DateStart.Value.ToUniversalTime());
			}

			if (request.Model.DateEnd.HasValue)
			{
				query = query.Where(x => x.DateStartedUtc < request.Model.DateEnd.Value.ToUniversalTime());
			}

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
				// Keep unstarted or completed at top, then by descending dates
				query = query
					.OrderBy(x => x.DateStartedUtc.HasValue)
					.ThenBy(x => x.DateCompletedUtc.HasValue)
					.ThenByDescending(x => x.DateCompletedUtc)
					.ThenByDescending(x => x.DateStartedUtc)
					.ThenByDescending(x => x.DateCreatedUtc);
			}

			query = query
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize);

			result.Data = await mapper.ProjectTo<WorkoutListModel>(query)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
