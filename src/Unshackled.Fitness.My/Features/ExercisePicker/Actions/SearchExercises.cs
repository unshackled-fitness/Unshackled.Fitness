using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;

namespace Unshackled.Fitness.My.Features.ExercisePicker.Actions;

public class SearchExercises
{
	public class Query : IRequest<SearchResult<ExerciseModel>>
	{
		public long MemberId { get; private set; }
		public SearchExerciseModel Model { get; private set; }

		public Query(long memberId, SearchExerciseModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<ExerciseModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<ExerciseModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<ExerciseModel>();
			var query = db.Exercises
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId && x.IsArchived == false);

			if (request.Model.MuscleType != MuscleTypes.Any)
			{
				query = query.Where(x => x.Muscles.Contains(request.Model.MuscleType.ToSearchString()));
			}

			if (request.Model.EquipmentType != EquipmentTypes.Any)
			{
				query = query.Where(x => x.Equipment.Contains(request.Model.EquipmentType.ToSearchString()));
			}

			if (!string.IsNullOrEmpty(request.Model.Title))
			{
				query = query.Where(x => x.Title.Contains(request.Model.Title));
			}

			result.Total = await query.CountAsync(cancellationToken);

			query = query
				.OrderBy(x => x.Title);

			query = query
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize);

			result.Data = await mapper.ProjectTo<ExerciseModel>(query)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
