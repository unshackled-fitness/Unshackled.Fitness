using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class SearchCompletedSets
{
	public class Query : IRequest<SearchResult<CompletedSetModel>>
	{
		public long MemberId { get; private set; }
		public SearchSetModel Model { get; private set; }

		public Query(long memberId, SearchSetModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<CompletedSetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<CompletedSetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<CompletedSetModel>(request.Model.PageSize);

			long exerciseId = request.Model.ExerciseSid.DecodeLong();
			if (exerciseId == 0)
				return result;

			long excludeWorkoutId = request.Model.ExcludeWorkoutSid.DecodeLong();

			var query = db.WorkoutSets
				.Include(x => x.Workout)
				.Include(x => x.Exercise)
				.AsNoTracking()
				.Where(x => x.ExerciseId == exerciseId 
					&& x.Exercise.MemberId == request.MemberId
					&& x.WorkoutId != excludeWorkoutId
					&& x.Workout.DateCompletedUtc != null
					&& x.DateRecordedUtc != null);

			if (request.Model.SetMetricType.HasReps() && request.Model.RepMode.HasValue)
			{
				query = query.Where(x => x.RepMode == request.Model.RepMode.Value);
			}

			var queryTarget = query;
			if (request.Model.SetMetricType.HasReps())
			{
				// Start query with target reps
				if (request.Model.RepsTarget.HasValue)
				{
					queryTarget = queryTarget.Where(x => x.RepsTarget == request.Model.RepsTarget.Value);
				}
				result.Total = await queryTarget.CountAsync(cancellationToken);
			}
			else
			{
				// Start query with target seconds
				if (request.Model.SecondsTarget.HasValue)
				{
					queryTarget = queryTarget.Where(x => x.SecondsTarget == request.Model.SecondsTarget.Value);
				}
				result.Total = await queryTarget.CountAsync(cancellationToken);
			}

			// If no results, try again without target
			if (result.Total == 0)
			{
				result.Total = await query.CountAsync(cancellationToken);
			}
			else // keep query with target
			{
				query = queryTarget;
			}

			query = query
				.OrderByDescending(x => x.Workout.DateCompletedUtc)
				.ThenBy(x => x.DateRecordedUtc);

			query = query
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize);

			result.Data = await mapper.ProjectTo<CompletedSetModel>(query)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
