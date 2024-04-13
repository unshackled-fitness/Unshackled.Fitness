using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class UpdateExercise
{
	public class Command : IRequest<CommandResult<ExerciseModel>>
	{
		public long MemberId { get; private set; }
		public FormExerciseModel Model { get; private set; }

		public Command(long memberId, FormExerciseModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<ExerciseModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<ExerciseModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			long exerciseId = request.Model.Sid.DecodeLong();

			if (exerciseId == 0)
				return new CommandResult<ExerciseModel>(false, "Invalid exercise.");

			var exercise = await db.Exercises
				.Where(e => e.Id == exerciseId && e.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (exercise is null)
				return new CommandResult<ExerciseModel>(false, "Invalid exercise.");

			var initSetMetricType = exercise.DefaultSetMetricType;

			exercise.DefaultSetMetricType = request.Model.DefaultSetMetricType;
			exercise.DefaultSetType = request.Model.DefaultSetType;
			exercise.Description = request.Model.Description?.Trim();
			exercise.Equipment = request.Model.Equipment.ToJoinedIntString();
			exercise.IsTrackingSplit = request.Model.IsTrackingSplit;
			exercise.Muscles = request.Model.Muscles.ToJoinedIntString();
			exercise.Title = request.Model.Title.ToUpper().Trim();
			await db.SaveChangesAsync(cancellationToken);

			return new CommandResult<ExerciseModel>(true, "Exercise updated.", mapper.Map<ExerciseModel>(exercise));
		}
	}
}