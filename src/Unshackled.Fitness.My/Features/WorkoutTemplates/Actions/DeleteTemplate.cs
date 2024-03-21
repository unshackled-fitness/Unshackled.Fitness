using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class DeleteTemplate
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public string TemplateSid { get; private set; }

		public Command(long memberId, string templateSid)
		{
			MemberId = memberId;
			TemplateSid = templateSid;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult> Handle(Command request, CancellationToken cancellationToken)
		{
			long templateId = request.TemplateSid.DecodeLong();

			if (templateId == 0)
				return new CommandResult<string>(false, "Invalid template ID.");

			var template = await db.WorkoutTemplates
				.Where(x => x.Id == templateId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync(cancellationToken);

			if (template == null)
				return new CommandResult(false, "Invalid template.");

			using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

			try
			{
				await db.WorkoutTemplateSets
					.Where(x => x.WorkoutTemplateId == template.Id)
					.DeleteFromQueryAsync(cancellationToken);

				await db.WorkoutTemplateSetGroups
					.Where(x => x.WorkoutTemplateId == template.Id)
					.DeleteFromQueryAsync(cancellationToken);

				await db.WorkoutTemplateTasks
					.Where(x => x.WorkoutTemplateId == template.Id)
					.DeleteFromQueryAsync(cancellationToken);

				await db.ProgramTemplates
					.Where(x => x.WorkoutTemplateId == template.Id)
					.DeleteFromQueryAsync(cancellationToken);

				db.WorkoutTemplates.Remove(template);
				await db.SaveChangesAsync(cancellationToken);

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Template deleted.");
			}
			catch
			{
				await transaction.RollbackAsync(cancellationToken);
				return new CommandResult(false, "Database error. Template could not be deleted.");
			}
		}
	}
}