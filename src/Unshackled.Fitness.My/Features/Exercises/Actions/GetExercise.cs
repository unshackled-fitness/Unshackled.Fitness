using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class GetExercise
{
	public class Query : IRequest<ExerciseModel>
	{
		public long Id { get; private set; }
		public long MemberId { get; private set; }

		public Query(long memberId, long id)
		{
			Id = id;
			MemberId = memberId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, ExerciseModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<ExerciseModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<ExerciseModel>(db.Exercises
				.AsNoTracking()
				.Where(x => x.Id == request.Id && x.MemberId == request.MemberId))
				.SingleOrDefaultAsync(cancellationToken) ?? new();
		}
	}
}