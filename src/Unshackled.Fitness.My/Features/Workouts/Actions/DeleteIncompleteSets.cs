using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class DeleteIncompleteSets
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string Sid { get; private set; }

		public Command(long memberId, string sid)
		{
			MemberId = memberId;
			Sid = sid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long workoutId = request.Sid.DecodeLong();
			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			var incompleteSetGroups = await db.WorkoutSets
				.Where(x => x.WorkoutId == workoutId && x.MemberId == request.MemberId && x.DateRecordedUtc == null)
				.Select(x => x.ListGroupId)
				.Distinct()
				.ToListAsync();

			try
			{
				await db.WorkoutSets
					.Where(x => x.WorkoutId == workoutId && x.MemberId == request.MemberId && x.DateRecordedUtc == null)
					.DeleteFromQueryAsync();

				foreach (var groupId in incompleteSetGroups)
				{
					int remainingSetsInGroup = await db.WorkoutSets
						.Where(x => x.WorkoutId == workoutId && x.ListGroupId == groupId)
					.CountAsync();
					
					int totalGroups = await db.WorkoutSetGroups
						.Where(x => x.WorkoutId == workoutId)
						.CountAsync();

					// Delete if no sets left in group and it is not the last remaining group in the workout
					if (remainingSetsInGroup == 0 && totalGroups > 1)
					{
						await db.WorkoutSetGroups
							.Where(x => x.Id == groupId)
							.DeleteFromQueryAsync();
					}
				}
				
				var recordedSets = await db.WorkoutSets
					.Where(x => x.WorkoutId == workoutId && x.MemberId == request.MemberId)
					.OrderBy(x => x.DateRecordedUtc)
					.ToListAsync();

				int sortOrder = 0;
				foreach(var recordedSet in recordedSets)
				{
					recordedSet.SortOrder = sortOrder;
					sortOrder++;
				}
				await db.SaveChangesAsync();

				await db.UpdateWorkoutStats(workoutId, request.MemberId);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Incomplete sets deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}