using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class SearchResults
{
	public class Query : IRequest<SearchResult<ResultListModel>>
	{
		public long MemberId { get; private set; }
		public SearchResultsModel Model { get; private set; }

		public Query(long memberId, SearchResultsModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<ResultListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<ResultListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<ResultListModel>(request.Model.PageSize);

			long exerciseId = request.Model.ExerciseSid.DecodeLong();
			if (exerciseId == 0)
				return result;

			var query = db.WorkoutSets
				.Include(x => x.Workout)
				.Include(x => x.Exercise)
				.AsNoTracking()
				.Where(x => x.ExerciseId == exerciseId 
					&& x.Exercise.MemberId == request.MemberId
					&& x.Workout.DateCompletedUtc != null
					&& x.DateRecordedUtc != null
					&& x.SetType == request.Model.SetType);

			if (request.Model.DateStart.HasValue)
			{
				query = query.Where(x => x.Workout.DateCompletedUtc >= request.Model.DateStart.Value.ToUniversalTime());
			}

			if (request.Model.DateEnd.HasValue)
			{
				query = query.Where(x => x.Workout.DateCompletedUtc <= request.Model.DateEnd.Value.ToUniversalTime());
			}

			if (request.Model.SetMetricType.HasReps() && request.Model.RepsTarget.HasValue)
			{
				query = query.Where(x => x.RepsTarget == request.Model.RepsTarget.Value);
			}
			else if (request.Model.SecondsTarget.HasValue)
			{
				query = query.Where(x => x.SecondsTarget == request.Model.SecondsTarget.Value);
			}

			result.Total = await query.CountAsync(cancellationToken);

			query = query
				.OrderByDescending(x => x.Workout.DateCompletedUtc)
				.ThenBy(x => x.DateRecordedUtc);

			query = query
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize);

			result.Data = await mapper.ProjectTo<ResultListModel>(query)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
