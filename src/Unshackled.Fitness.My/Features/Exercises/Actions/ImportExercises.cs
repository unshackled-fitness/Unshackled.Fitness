using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;

namespace Unshackled.Fitness.My.Features.Exercises.Actions;

public class ImportExercises
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public List<LibraryListModel> SelectedExercises { get; private set; }

		public Command(long memberId, List<LibraryListModel> selectedExercises)
		{
			MemberId = memberId;
			SelectedExercises = selectedExercises;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			if (request.SelectedExercises.Count > AppGlobals.MaxSelectionLimit)
				return new CommandResult(false, "Maximum selection size exceeded.");

			if (request.SelectedExercises == null)
				return new CommandResult(false, "No exercises were selected.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				int countAdded = 0;
				int countSkipped = 0;
				foreach (var item in request.SelectedExercises)
				{
					bool exists = await db.Exercises
						.Where(x => x.MatchId == item.MatchId && x.MemberId == request.MemberId)
						.AnyAsync();

					if (!exists)
					{
						var entity = new ExerciseEntity
						{
							DefaultSetMetricType = item.DefaultSetMetricType,
							DefaultSetType = item.DefaultSetType,
							Description = item.Description,
							Equipment = item.Equipment.ToJoinedIntString(),
							IsArchived = false,
							IsTrackingSplit = item.IsTrackingSplit,
							MatchId = item.MatchId,
							MemberId = request.MemberId,
							Muscles = item.Muscles.ToJoinedIntString(),
							Title = item.Title
						};
						db.Exercises.Add(entity);
						await db.SaveChangesAsync();
						countAdded++;
					}
					else
					{
						countSkipped++;
					}
				}

				await transaction.CommitAsync(cancellationToken);

				string msg = countAdded != 1 ? $"{countAdded} exercises added." : "1 exercise added.";
				if (countSkipped > 0)
				{
					msg += countSkipped != 1 ? $" {countSkipped} exercises were already in your library." : " 1 exercise was already in your library.";
				}

				return new CommandResult(true, msg);
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "Could not insert the exercises.");
			}
		}
	}
}