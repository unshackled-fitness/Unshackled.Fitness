using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class UpdateTemplateTasks
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public long TemplateId { get; private set; }
		public UpdateTemplateTasksModel Model { get; private set; }

		public Command(long memberId, long templateId, UpdateTemplateTasksModel model)
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

			var currentTasks = await db.WorkoutTemplateTasks
				.Where(x => x.WorkoutTemplateId == request.TemplateId)
				.ToListAsync();

			using var transaction = await db.Database.BeginTransactionAsync();

			try
			{
				// Update template tasks
				foreach (var task in request.Model.Tasks)
				{
					// No ID, so new
					if (string.IsNullOrEmpty(task.Sid))
					{
						db.WorkoutTemplateTasks.Add(new WorkoutTemplateTaskEntity
						{
							MemberId = request.MemberId,
							SortOrder = task.SortOrder,
							Text = task.Text,
							Type = task.Type,
							WorkoutTemplateId = request.TemplateId
						});
					}
					// Has ID
					else
					{
						// Find existing
						var existing = currentTasks
							.Where(x => x.Id == task.Sid.DecodeLong())
							.SingleOrDefault();

						if (existing != null)
						{
							existing.SortOrder = task.SortOrder;
							existing.Text = task.Text;
							existing.Type = task.Type;
						}
					}					
				}

				// Delete sets
				foreach (var task in request.Model.DeletedTasks)
				{
					// Find existing
					var existing = currentTasks
						.Where(x => x.Id == task.Sid.DecodeLong())
						.SingleOrDefault();

					if (existing != null)
					{
						db.WorkoutTemplateTasks.Remove(existing);
					}
				}

				// Save set changes
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Tasks updated.");
			}
			catch
			{
				return new CommandResult(false, "An error occurred while updating your tasks.");
			}
		}
	}
}