using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class DuplicateTemplate
{
	public class Command : IRequest<CommandResult<string>>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }
		public FormTemplateModel Model { get; private set; }

		public Command(long memberId, long templateId, FormTemplateModel model)
		{
			MemberId = memberId;
			TemplateId = templateId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<string>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<string>> Handle(Command request, CancellationToken cancellationToken)
		{
			var template = await db.WorkoutTemplates
				.AsNoTracking()
				.Where(x => x.Id == request.TemplateId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (template == null)
				return new CommandResult<string>(false, "Invalid template.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				// Copy template
				var duplicate = new WorkoutTemplateEntity
				{
					Description = request.Model.Description,
					ExerciseCount = template.ExerciseCount,
					MemberId = request.MemberId,
					MusclesTargeted = template.MusclesTargeted,
					SetCount = template.SetCount,
					Title = request.Model.Title
				};
				db.WorkoutTemplates.Add(duplicate);
				await db.SaveChangesAsync(cancellationToken);

				// Create map of template group ids to workout group ids
				Dictionary<long, long> groupIdMap = new();

				// Copy groups
				var groups = await db.WorkoutTemplateSetGroups
					.AsNoTracking()
					.Where(x => x.WorkoutTemplateId == request.TemplateId)
					.ToListAsync();

				foreach ( var group in groups)
				{
					WorkoutTemplateSetGroupEntity g = new()
					{
						MemberId = group.MemberId,
						SortOrder = group.SortOrder,
						Title = group.Title,
						WorkoutTemplateId = duplicate.Id
					};
					db.WorkoutTemplateSetGroups.Add(g);
					await db.SaveChangesAsync(cancellationToken);

					groupIdMap.Add(group.Id, g.Id);
				}

				// Copy sets
				var sets = await db.WorkoutTemplateSets
					.AsNoTracking()
					.Where(x => x.WorkoutTemplateId == request.TemplateId)
					.Select(x => new WorkoutTemplateSetEntity
					{
						ExerciseId = x.ExerciseId,
						ListGroupId = groupIdMap[x.ListGroupId],
						MemberId = x.MemberId,
						RepMode = x.RepMode,
						RepsTarget = x.RepsTarget,
						SetType = x.SetType,
						SortOrder = x.SortOrder,
						WorkoutTemplateId = duplicate.Id
					})
					.ToListAsync();

				db.WorkoutTemplateSets.AddRange(sets);
				await db.SaveChangesAsync(cancellationToken);

				// Copy Tasks
				var tasks = await db.WorkoutTemplateTasks
					.AsNoTracking()
					.Where(x => x.WorkoutTemplateId == request.TemplateId)
					.Select(x => new WorkoutTemplateTaskEntity
					{
						MemberId = x.MemberId,
						SortOrder = x.SortOrder,
						Text = x.Text,
						Type = x.Type,
						WorkoutTemplateId = duplicate.Id
					})
					.ToListAsync();

				db.WorkoutTemplateTasks.AddRange(tasks);
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult<string>(true, "Template duplicated.", duplicate.Id.Encode());
			}
			catch
			{
				return new CommandResult<string>(false, Globals.UnexpectedError);
			}
		}
	}
}