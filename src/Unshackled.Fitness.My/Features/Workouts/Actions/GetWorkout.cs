using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Extensions;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class GetWorkout
{
	public class Query : IRequest<FormWorkoutModel>
	{
		public long MemberId { get; private set; }
		public long WorkoutId { get; private set; }

		public Query(long memberId, long workoutId)
		{
			MemberId = memberId;
			WorkoutId = workoutId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, FormWorkoutModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<FormWorkoutModel> Handle(Query request, CancellationToken cancellationToken)
		{
			var member = await db.Members
				.AsNoTracking()
				.Where(s => s.Id == request.MemberId)
				.SingleOrDefaultAsync();

			if(member == null)
				return new FormWorkoutModel();

			var workout = await mapper.ProjectTo<FormWorkoutModel>(db.Workouts
				.AsNoTracking()
				.Where(x => x.Id == request.WorkoutId && x.MemberId == request.MemberId))
				.SingleOrDefaultAsync() ?? new();

			workout.Tasks = await mapper.ProjectTo<FormWorkoutTaskModel>(db.WorkoutTasks
				.AsNoTracking()
				.Where(x => x.WorkoutId == request.WorkoutId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();

			workout.Groups = await mapper.ProjectTo<FormWorkoutSetGroupModel>(db.WorkoutSetGroups
				.AsNoTracking()
				.Where(x => x.WorkoutId == request.WorkoutId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();

			UnitSystems defaultUnits = (await db.GetMemberSettings(member.Id)).DefaultUnits;

			workout.Sets = (await db.WorkoutSets
				.Include(x => x.Exercise)
				.AsNoTracking()
				.Where(x => x.WorkoutId == request.WorkoutId)
				.OrderBy(x => x.SortOrder)
				.ToListAsync())
				.Map(defaultUnits);

			return workout;
		}
	}
}
