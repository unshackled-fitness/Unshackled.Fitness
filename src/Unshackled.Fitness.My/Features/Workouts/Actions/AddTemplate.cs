using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts.Actions;

public class AddTemplate
{
	public class Command : IRequest<CommandResult<string>>
	{
		public long MemberId { get; private set; }
		public FormAddTemplateModel Model { get; private set; }

		public Command(long memberId, FormAddTemplateModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			long workoutId = request.Model.WorkoutSid.DecodeLong();

			if(workoutId == 0)
				return new CommandResult<string>(false, "Invalid workout ID.");

			var workout = await db.Workouts
				.Where(x => x.Id == workoutId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (workout == null)
				return new CommandResult<string>(false, "Previous workout not found.");

			WorkoutTemplateEntity template = new()
			{
				MemberId = request.MemberId,
				Title = request.Model.Title,
				Description = request.Model.Description,
				ExerciseCount = workout.ExerciseCount,
				SetCount = workout.SetCount
			};

			try
			{
				// Save so we get new template ID
				db.WorkoutTemplates.Add(template);
				await db.SaveChangesAsync(cancellationToken);

				// Get workout groups
				var workoutGroups = await db.WorkoutSetGroups
					.AsNoTracking()
					.Where(x => x.WorkoutId == workout.Id)
					.OrderBy(x => x.SortOrder)
					.ToListAsync();

				// Create map of prev group ids to new group ids
				Dictionary<long, long> groupIdMap = new();

				// Create template groups
				foreach (var group in workoutGroups)
				{
					// Add group with new template ID
					WorkoutTemplateSetGroupEntity g = new()
					{
						MemberId = request.MemberId,
						SortOrder = group.SortOrder,
						Title = group.Title,
						WorkoutTemplateId = template.Id
					};

					// Save so we get new set ID
					db.WorkoutTemplateSetGroups.Add(g);
					await db.SaveChangesAsync(cancellationToken);

					// Add to map
					groupIdMap.Add(group.Id, g.Id);
				}

				// Get workout sets
				var workoutSets = await db.WorkoutSets
					.AsNoTracking()
					.Where(x => x.WorkoutId == workout.Id)
					.OrderBy(x => x.SortOrder)
					.ToListAsync();

				// Create template sets
				foreach (var set in workoutSets)
				{
					// Add set with new template ID
					WorkoutTemplateSetEntity s = new()
					{
						ExerciseId = set.ExerciseId,
						SetMetricType = set.SetMetricType,
						ListGroupId = groupIdMap[set.ListGroupId],
						MemberId = set.MemberId,
						RepMode = set.RepMode,
						RepsTarget = set.RepsTarget,
						SecondsTarget = set.SecondsTarget,
						SetType = set.SetType,
						SortOrder = set.SortOrder,
						WorkoutTemplateId = template.Id
					};

					db.WorkoutTemplateSets.Add(s);
				}
				await db.SaveChangesAsync(cancellationToken);

				// Get workouts tasks
				var tasks = await db.WorkoutTasks
					.AsNoTracking()
					.Where(x => x.WorkoutId == workout.Id)
					.OrderBy(x => x.Type)
						.ThenBy(x => x.SortOrder)
					.Select(x => new WorkoutTemplateTaskEntity
					{
						MemberId = x.MemberId,
						SortOrder = x.SortOrder,
						Text = x.Text,
						Type = x.Type,
						WorkoutTemplateId = template.Id
					})
					.ToListAsync();

				if (tasks.Any())
				{
					db.WorkoutTemplateTasks.AddRange(tasks);
					await db.SaveChangesAsync(cancellationToken);
				}

				await transaction.CommitAsync(cancellationToken);
				return new CommandResult<string>(true, "Template created.", template.Id.Encode());
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult<string>(false, Globals.UnexpectedError);
			}
		}
	}
}