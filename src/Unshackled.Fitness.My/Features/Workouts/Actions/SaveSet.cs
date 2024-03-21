using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class SaveSet
{
	public class Command : IRequest<CommandResult<FormWorkoutSetModel>>
	{
		public long MemberId { get; private set; }
		public FormWorkoutSetModel Set { get; private set; }

		public Command(long memberId, FormWorkoutSetModel set)
		{
			MemberId = memberId;
			Set = set;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<FormWorkoutSetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<FormWorkoutSetModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			var member = await db.Members
				.AsNoTracking()
				.Where(s => s.Id == request.MemberId)
				.SingleOrDefaultAsync();

			if (member == null)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid member.");

			long setId = request.Set.Sid.DecodeLong();

			var set = await db.WorkoutSets
				.Include(x => x.Workout)
				.Include(x => x.Exercise)
				.Where(x => x.Id == setId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (set == null)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid workout set.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				if (!set.DateRecordedUtc.HasValue)
					set.DateRecordedUtc = DateTime.UtcNow;

				bool needsWgt = request.Set.SetMetricType == SetMetricTypes.WeightReps || request.Set.SetMetricType == SetMetricTypes.WeightTime;
				bool needsReps = request.Set.SetMetricType == SetMetricTypes.WeightReps || request.Set.SetMetricType == SetMetricTypes.RepsOnly;
				bool needsTime = request.Set.SetMetricType == SetMetricTypes.WeightTime || request.Set.SetMetricType == SetMetricTypes.TimeOnly;

				decimal wgt = needsWgt && request.Set.Weight.HasValue ? request.Set.Weight.Value : 0;
				int reps = needsReps && request.Set.Reps.HasValue ? request.Set.Reps.Value : 0;
				int repsLeft = needsReps && request.Set.RepsLeft.HasValue ? request.Set.RepsLeft.Value : 0;
				int repsRight = needsReps && request.Set.RepsRight.HasValue ? request.Set.RepsRight.Value : 0;
				int seconds = needsTime ? request.Set.Seconds : 0;
				int secondsLeft = needsTime ? request.Set.SecondsLeft : 0;
				int secondsRight = needsTime ? request.Set.SecondsRight : 0;

				set.SetMetricType = request.Set.SetMetricType;
				set.RepMode = request.Set.RepMode;
				set.Reps = reps;
				set.RepsLeft = repsLeft;
				set.RepsRight = repsRight;
				set.RepsTarget = request.Set.RepsTarget;
				set.SetType = request.Set.SetType;
				set.Seconds = seconds;
				set.SecondsLeft = secondsLeft;
				set.SecondsRight = secondsRight;
				set.SecondsTarget = request.Set.SecondsTarget;
				set.IntensityTarget = request.Set.IntensityTarget;

				switch (request.Set.WeightUnit)
				{
					case WeightUnits.kg:
						set.WeightKg = wgt;
						set.WeightLb = Math.Round(wgt * WeightUnits.lb.ConversionFactor(), 3);
						break;
					case WeightUnits.lb:
						set.WeightLb = wgt;
						set.WeightKg = Math.Round(wgt / WeightUnits.lb.ConversionFactor(), 3);
						break;
					default:
						return new CommandResult<FormWorkoutSetModel>(false, "Invalid weight unit.");
				}

				set.VolumeKg = set.IsTrackingSplit
					? Calculator.Volume(set.WeightKg, set.RepsLeft, set.RepsRight)
					: Calculator.Volume(set.WeightKg, set.Reps);
				set.VolumeLb = set.IsTrackingSplit
					? Calculator.Volume(set.WeightLb, set.RepsLeft, set.RepsRight)
					: Calculator.Volume(set.WeightLb, set.Reps);

				await db.SaveChangesAsync();

				// Recalculate best sets and PR's if workout is complete
				if (set.Workout.DateCompletedUtc.HasValue)
					await db.UpdateWorkoutRecords(set.WorkoutId);

				await db.UpdateWorkoutStats(set.WorkoutId, request.MemberId);

				await transaction.CommitAsync(cancellationToken);

				UnitSystems defaultUnits = (await db.GetMemberSettings(member.Id)).DefaultUnits;

				return new CommandResult<FormWorkoutSetModel>(true, "Set saved.", set.Map(defaultUnits));
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult<FormWorkoutSetModel>(false, Globals.UnexpectedError);
			}
		}
	}
}