using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class ListPersonalRecords
{
	public class Query : IRequest<List<RecordListModel>>
	{
		public long MemberId { get; private set; }
		public long Id { get; private set; }

		public Query(long memberId, long id)
		{
			MemberId = memberId;
			Id = id;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<RecordListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<RecordListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			List<RecordListModel> list = new();

			// Weight
			var pr = await mapper.ProjectTo<RecordListModel>(db.WorkoutSets
				.AsNoTracking()
				.Include(x => x.Workout)
				.Where(x => x.MemberId == request.MemberId
					&& x.ExerciseId == request.Id
					&& x.Workout.DateCompletedUtc != null
					&& x.IsRecordWeight == true
					&& x.WeightKg > 0)
				.OrderByDescending(x => x.DateRecordedUtc))
				.FirstOrDefaultAsync();

			if (pr != null)
			{
				pr.RecordType = RecordListModel.RecordTypes.Weight;
				list.Add(pr);
			}

			// Volume
			pr = await mapper.ProjectTo<RecordListModel>(db.WorkoutSets
				.AsNoTracking()
				.Include(x => x.Workout)
				.Where(x => x.MemberId == request.MemberId
					&& x.ExerciseId == request.Id
					&& x.Workout.DateCompletedUtc != null
					&& x.IsRecordVolume == true
					&& x.VolumeKg > 0)
				.OrderByDescending(x => x.DateRecordedUtc))
				.FirstOrDefaultAsync();

			if (pr != null)
			{
				pr.RecordType = RecordListModel.RecordTypes.Volume;
				list.Add(pr);
			}

			// Time
			pr = await mapper.ProjectTo<RecordListModel>(db.WorkoutSets
				.AsNoTracking()
				.Include(x => x.Workout)
				.Where(x => x.MemberId == request.MemberId
					&& x.ExerciseId == request.Id
					&& x.Workout.DateCompletedUtc != null
					&& x.IsRecordSeconds == true
					&& x.WeightKg == 0
					&& x.WeightLb == 0
					&& (x.Seconds > 0 || x.SecondsLeft > 0 || x.SecondsRight > 0))
				.OrderByDescending(x => x.DateRecordedUtc))
				.FirstOrDefaultAsync();

			if (pr != null)
			{
				pr.RecordType = RecordListModel.RecordTypes.Time;
				list.Add(pr);
			}

			// Time at Weight
			pr = await mapper.ProjectTo<RecordListModel>(db.WorkoutSets
				.AsNoTracking()
				.Include(x => x.Workout)
				.Where(x => x.MemberId == request.MemberId
					&& x.ExerciseId == request.Id
					&& x.Workout.DateCompletedUtc != null
					&& x.IsRecordSecondsAtWeight == true
					&& x.WeightKg > 0
					&& x.WeightLb > 0
					&& (x.Seconds > 0 || x.SecondsLeft > 0 || x.SecondsRight > 0))
				.OrderByDescending(x => x.DateRecordedUtc))
				.FirstOrDefaultAsync();

			if (pr != null)
			{
				pr.RecordType = RecordListModel.RecordTypes.WeightTime;
				list.Add(pr);
			}

			return list;
		}
	}
}