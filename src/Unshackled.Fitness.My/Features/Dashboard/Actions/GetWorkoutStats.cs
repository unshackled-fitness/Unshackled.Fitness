using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Features.Dashboard.Actions;

public class GetWorkoutStats
{
	public class Query : IRequest<WorkoutStatsModel>
	{
		public long MemberId { get; private set; }
		public DateTime ToDateUtc { get; private set; }

		public Query(long memberId, DateTime toDateUtc)
		{
			MemberId = memberId;
			ToDateUtc = toDateUtc;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, WorkoutStatsModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<WorkoutStatsModel> Handle(Query request, CancellationToken cancellationToken)
		{
			DateTime toDateUtc = request.ToDateUtc;
			DateTime fromDateUtc = toDateUtc.AddYears(-1);

			var model = new WorkoutStatsModel();
			model.ToDateUtc = request.ToDateUtc;

			model.Workouts = await mapper.ProjectTo<WorkoutModel>(db.Workouts
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId && x.DateCompletedUtc > fromDateUtc && x.DateCompletedUtc <= toDateUtc)
				.OrderBy(x => x.DateCompletedUtc))
				.ToListAsync();

			model.Years = await db.Workouts
				.Where(x => x.MemberId == request.MemberId && x.DateCompletedUtc.HasValue)
				.Select(x => x.DateCompletedUtc!.Value.Year)
				.Distinct()
				.ToListAsync();

			model.TotalWorkouts = await db.Workouts
				.Where(x => x.MemberId == request.MemberId && x.DateCompletedUtc.HasValue)
				.CountAsync();

			var totals = await db.Workouts
				.Where(x => x.MemberId == request.MemberId && x.DateCompletedUtc.HasValue)
				.GroupBy(x => true)
				.Select(x => new
				{
					VolumeKg = x.Sum(y => y.VolumeKg),
					VolumeLb = x.Sum(y => y.VolumeLb)
				})
				.SingleOrDefaultAsync();

			if (totals != null)
			{
				model.TotalVolumeLb = totals.VolumeLb;
				model.TotalVolumeKg = totals.VolumeKg;
			}

			return model;
		}
	}
}
