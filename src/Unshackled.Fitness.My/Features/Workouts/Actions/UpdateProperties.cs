using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class UpdateProperties
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public FormPropertiesModel Model { get; private set; }

		public Command(long memberId, FormPropertiesModel model)
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
			long workoutId = request.Model.Sid.DecodeLong();

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult(false, "Invalid workout.");

			// Update workout
			workout.Title = request.Model.Title.Trim();
			workout.DateStartedUtc = request.Model.DateStartedUtc;
			workout.DateCompletedUtc = request.Model.DateCompletedUtc;
			workout.Rating = request.Model.Rating;
			workout.Notes = request.Model.Notes;
			await db.SaveChangesAsync(cancellationToken);

			return new CommandResult(true, "Workout updated.");
		}
	}
}