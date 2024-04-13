using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class DuplicateSet
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
				.AsNoTracking()
				.Include(x => x.Workout)
				.Include(x => x.Exercise)
				.Where(x => x.Id == setId && x.Workout.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (set == null)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid workout set.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Adjust sort order for other sets in the workout with higher sort orders
				await db.WorkoutSets
					.Where(x => x.WorkoutId == set.WorkoutId && x.SortOrder > set.SortOrder)
					.UpdateFromQueryAsync(x => new WorkoutSetEntity { SortOrder = x.SortOrder + 1 });

				WorkoutSetEntity newSet = new()
				{
					ExerciseId = set.ExerciseId,
					SetMetricType = set.SetMetricType,
					ListGroupId = set.ListGroupId,
					IntensityTarget = request.Set.IntensityTarget,
					IsTrackingSplit = set.IsTrackingSplit,
					MemberId = set.MemberId,
					RepMode = request.Set.RepMode,
					RepsTarget = request.Set.RepsTarget,
					SecondsTarget = request.Set.SecondsTarget,
					SetType = request.Set.SetType,
					SortOrder = set.SortOrder + 1,
					WorkoutId = set.WorkoutId
				};
				db.WorkoutSets.Add(newSet);
				await db.SaveChangesAsync();

				await db.UpdateWorkoutStats(set.WorkoutId, request.MemberId);

				await transaction.CommitAsync();

				newSet.Exercise = await db.Exercises
					.Where(x => x.Id == newSet.ExerciseId)
					.SingleOrDefaultAsync() ?? new();

				UnitSystems defaultUnits = (await db.GetMemberSettings(member.Id)).DefaultUnits;

				return new CommandResult<FormWorkoutSetModel>(true, "Set saved.", newSet.Map(defaultUnits));
			}
			catch
			{
				await transaction.RollbackAsync();
				return new CommandResult<FormWorkoutSetModel>(false, Globals.UnexpectedError);
			}
		}
	}
}