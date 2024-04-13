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

public class AddSet
{
	public class Command : IRequest<CommandResult<FormWorkoutSetModel>>
	{
		public long MemberId { get; private set; }
		public FormWorkoutSetModel Model { get; private set; }

		public Command(long memberId, FormWorkoutSetModel model)
		{
			MemberId = memberId;
			Model = model;
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

			var workout = await db.Workouts
				.Where(x => x.Id == request.Model.WorkoutSid.DecodeLong() && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid workout.");

			long exerciseId = request.Model.ExerciseSid.DecodeLong();

			if (exerciseId == 0)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid exercise.");

			long groupId = request.Model.ListGroupSid.DecodeLong();

			if (groupId == 0)
				return new CommandResult<FormWorkoutSetModel>(false, "Invalid set group.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				WorkoutSetEntity newSet = new()
				{
					ExerciseId = exerciseId,
					SetMetricType = request.Model.SetMetricType,
					ListGroupId = groupId,
					IntensityTarget = request.Model.IntensityTarget,
					IsTrackingSplit = request.Model.IsTrackingSplit,
					MemberId = request.MemberId,
					RepMode = request.Model.RepMode,
					RepsTarget = request.Model.RepsTarget,
					SecondsTarget = request.Model.SecondsTarget,
					SetType = request.Model.SetType,
					SortOrder = request.Model.SortOrder,
					WorkoutId = workout.Id
				};
				db.WorkoutSets.Add(newSet);
				await db.SaveChangesAsync();

				await db.UpdateWorkoutStats(workout.Id, request.MemberId);

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