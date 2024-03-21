using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class ListMergeExercises
{
	public class Query : IRequest<List<MergeExerciseModel>>
	{
		public long MemberId { get; private set; }
		public List<string> UIds { get; private set; }

		public Query(long memberId, List<string> uids)
		{
			MemberId = memberId;
			UIds = uids;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<MergeExerciseModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<MergeExerciseModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			List<MergeExerciseModel> models = new();
			var ids = request.UIds.DecodeLong();
			if (ids.Any())
			{
				models.AddRange(await mapper.ProjectTo<MergeExerciseModel>(db.Exercises
					.AsNoTracking()
					.Where(x => ids.Contains(x.Id) && x.MemberId == request.MemberId))
					.ToListAsync());
			}
			return models;
		}
	}
}