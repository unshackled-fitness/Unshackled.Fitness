using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class SaveExerciseNote
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public ExerciseNoteModel Model { get; private set; }

		public Command(long memberId, ExerciseNoteModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long exerciseId = request.Model.Sid.DecodeLong();

			var exercise = await db.Exercises
				.Where(x => x.Id == exerciseId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (exercise == null)
				return new CommandResult(false, "Invalid exercise.");

			// Update exercise
			exercise.Notes = request.Model.Notes?.Trim();
			await db.SaveChangesAsync(cancellationToken);

			return new CommandResult(true, "Note updated.");
		}
	}
}