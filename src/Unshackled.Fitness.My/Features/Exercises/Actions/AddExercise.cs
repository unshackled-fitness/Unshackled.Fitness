using AutoMapper;
using MediatR;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class AddExercise
{
	public class Command : IRequest<CommandResult<string>>
	{
		public long MemberId { get; private set; }
		public FormExerciseModel Model { get; private set; }

		public Command(long memberId, FormExerciseModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(BaseDbContext db, IMapper map) : base(db, map) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			var exercise = new ExerciseEntity
			{
				DefaultSetType = request.Model.DefaultSetType,
				DefaultSetMetricType = request.Model.DefaultSetMetricType,
				Description = request.Model.Description?.Trim(),
				Equipment = request.Model.Equipment.ToJoinedIntString(),
				IsTrackingSplit = request.Model.IsTrackingSplit,
				MemberId = request.MemberId,
				Muscles = request.Model.Muscles.ToJoinedIntString(),
				Title = request.Model.Title.ToUpper().Trim(),				
			};
			db.Exercises.Add(exercise);
			await db.SaveChangesAsync(cancellationToken);
			return new CommandResult<string>(true, "Exercise added.", exercise.Id.Encode());
		}
	}
}