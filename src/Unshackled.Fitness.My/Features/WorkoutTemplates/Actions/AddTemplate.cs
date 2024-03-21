using AutoMapper;
using MediatR;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class AddTemplate
{
	public class Command : IRequest<CommandResult<string>>
	{
		public long MemberId { get; private set; }
		public FormTemplateModel Model { get; private set; }

		public Command(long memberId, FormTemplateModel model)
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
			try
			{
				WorkoutTemplateEntity template = new()
				{
					Description = request.Model.Description?.Trim(),
					MemberId = request.MemberId,
					Title = request.Model.Title.Trim()
				};
				db.WorkoutTemplates.Add(template);
				await db.SaveChangesAsync(cancellationToken);

				WorkoutTemplateSetGroupEntity group = new()
				{
					WorkoutTemplateId = template.Id,
					MemberId = request.MemberId,
					SortOrder = 0,
					Title = string.Empty
				};
				db.WorkoutTemplateSetGroups.Add(group);
				await db.SaveChangesAsync(cancellationToken);

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