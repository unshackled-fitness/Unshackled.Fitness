using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class UpdateTemplateSets
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }
		public UpdateTemplateSetsModel Model { get; private set; }

		public Command(long memberId, long templateId, UpdateTemplateSetsModel model)
		{
			MemberId = memberId;
			TemplateId = templateId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			var template = await db.WorkoutTemplates
				.Where(x => x.Id == request.TemplateId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (template == null)
				return new CommandResult(false, "Invalid template.");

			var currentGroups = await db.WorkoutTemplateSetGroups
				.Where(x => x.WorkoutTemplateId == request.TemplateId)
				.ToListAsync();

			var currentSets = await db.WorkoutTemplateSets
				.Where(x => x.WorkoutTemplateId == request.TemplateId)
				.ToListAsync();

			using var transaction = await db.Database.BeginTransactionAsync();

			try
			{
				// Create map of string ids to long ids
				Dictionary<string, long> groupIdMap = currentGroups
					.Select(x => x.Id)
					.ToDictionary(x => x.Encode());

				// Add new groups first
				foreach (var group in request.Model.Groups
					.Where(x => x.IsNew == true)
					.ToList())
				{
					WorkoutTemplateSetGroupEntity gEntity = new()
					{
						WorkoutTemplateId = request.TemplateId,
						MemberId = request.MemberId,
						SortOrder = group.SortOrder,
						Title = group.Title.Trim()
					};
					db.WorkoutTemplateSetGroups.Add(gEntity);
					await db.SaveChangesAsync();

					groupIdMap.Add(group.Sid, gEntity.Id);
				}

				// Update template sets
				foreach (var set in request.Model.Sets)
				{
					// No ID, add new
					if (string.IsNullOrEmpty(set.Sid))
					{
						db.WorkoutTemplateSets.Add(new WorkoutTemplateSetEntity
						{
							ExerciseId = set.ExerciseSid.DecodeLong(),
							SetMetricType = set.SetMetricType,
							IntensityTarget = set.IntensityTarget,
							ListGroupId = groupIdMap[set.ListGroupSid],
							MemberId = request.MemberId,
							RepMode = set.RepMode,
							RepsTarget = set.RepsTarget,
							SecondsTarget = set.SecondsTarget,
							SetType = set.SetType,
							SortOrder = set.SortOrder,
							WorkoutTemplateId = request.TemplateId
						});

						await db.SaveChangesAsync(cancellationToken);
					}
					// Has ID, update
					else if (!string.IsNullOrEmpty(set.Sid))
					{
						// Find existing
						var existing = currentSets
							.Where(x => x.Id == set.Sid.DecodeLong())
							.SingleOrDefault();

						if (existing != null)
						{
							existing.SetMetricType = set.SetMetricType;
							existing.IntensityTarget = set.IntensityTarget;
							existing.ListGroupId = groupIdMap[set.ListGroupSid];
							existing.RepMode = set.RepMode;
							existing.RepsTarget = set.RepsTarget;
							existing.SecondsTarget = set.SecondsTarget;
							existing.SetType = set.SetType;
							existing.SortOrder = set.SortOrder;

							await db.SaveChangesAsync(cancellationToken);
						}
					}
				}

				// Delete sets
				foreach (var set in request.Model.DeletedSets)
				{
					// Find existing
					var existing = currentSets
						.Where(x => x.Id == set.Sid.DecodeLong())
						.SingleOrDefault();

					if (existing != null)
					{
						db.WorkoutTemplateSets.Remove(existing);
					}
					await db.SaveChangesAsync(cancellationToken);
				}

				// Update non-new groups
				foreach (var group in request.Model.Groups
					.Where(x => x.IsNew == false)
					.ToList())
				{
					var existing = currentGroups
						.Where(x => x.Id == group.Sid.DecodeLong())
						.SingleOrDefault();

					if (existing != null)
					{
						existing.SortOrder = group.SortOrder;
						existing.Title = group.Title.Trim();

						await db.SaveChangesAsync(cancellationToken);
					}
				}

				// Delete groups
				foreach (var group in request.Model.DeletedGroups)
				{
					var g = currentGroups.Where(x => x.Id == group.Sid.DecodeLong())
						.SingleOrDefault();

					if (g == null) continue;

					// Check any sets that might still be in group (should be 0 at this point)
					bool stopDelete = await db.WorkoutTemplateSets
						.Where(x => x.ListGroupId == g.Id)
						.AnyAsync();

					if (!stopDelete)
					{
						db.WorkoutTemplateSetGroups.Remove(g);
						await db.SaveChangesAsync();
					}
				}

				// *** Update set stats ***

				// Get set exercise data
				var model = await db.WorkoutTemplateSets
					.AsNoTracking()
					.Include(x => x.Exercise)
					.Where(x => x.WorkoutTemplateId == request.TemplateId)
					.Select(x => new
					{
						x.Id,
						x.ExerciseId,
						x.Exercise.Muscles
					})
					.ToListAsync();

				// Exercise count
				template.ExerciseCount = model.Select(x => x.ExerciseId).Distinct().Count();
				// Set count
				template.SetCount = model.Count;

				List<MuscleTypes> muscles = new();
				foreach (var exercise in model)
				{
					muscles.AddRange(EnumUtils.FromJoinedIntString<MuscleTypes>(exercise.Muscles));
				}				

				// Distinct muscles targeted
				string[] musclesTargeted = muscles
					.OrderBy(x => x)
					.Select(x => x.Title())
					.Distinct().ToArray();

				if (musclesTargeted.Length > 0)
				{
					template.MusclesTargeted = String.Join(", ", musclesTargeted);
				}
				else
				{
					template.MusclesTargeted = null;
				}

				// save template changes
				await db.SaveChangesAsync();

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Sets updated.");
			}
			catch
			{
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}