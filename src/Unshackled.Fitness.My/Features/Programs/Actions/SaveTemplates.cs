using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class SaveTemplates
{
	public class Command : IRequest<CommandResult>
	{
		public long MemberId { get; private set; }
		public FormUpdateTemplatesModel Model { get; private set; }

		public Command(long memberId, FormUpdateTemplatesModel model)
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
			long programId = request.Model.ProgramSid.DecodeLong();

			var program = await db.Programs
				.Where(x => x.Id == programId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (program == null)
				return new CommandResult(false, "Invalid program.");

			var currentTemplates = await db.ProgramTemplates
				.Where(x => x.ProgramId == program.Id)
				.OrderBy(x => x.SortOrder)
				.ToListAsync();

			using var transaction = await db.Database.BeginTransactionAsync();

			try
			{
				// Update program
				program.LengthWeeks = request.Model.LengthWeeks;

				// Remove from calendar if no templates are present
				if (!request.Model.Templates.Any())
				{
					program.NextTemplateIndex = 0;
					program.DateStarted = null;
				}

				await db.SaveChangesAsync(cancellationToken);

				// Update program templates
				foreach (var template in request.Model.Templates)
				{
					// Add new
					if (template.IsNew)
					{
						db.ProgramTemplates.Add(new ProgramTemplateEntity
						{
							DayNumber = template.DayNumber,
							MemberId = request.MemberId,
							ProgramId = program.Id,
							SortOrder = template.SortOrder,
							WeekNumber = template.WeekNumber,
							WorkoutTemplateId = template.WorkoutTemplateSid.DecodeLong()
						});

						await db.SaveChangesAsync(cancellationToken);
					}
					// Update
					else
					{
						// Find existing
						var existing = currentTemplates
							.Where(x => x.Id == template.Sid.DecodeLong())
							.SingleOrDefault();

						if (existing != null)
						{
							existing.SortOrder = template.SortOrder;
							existing.WeekNumber = template.WeekNumber;
							await db.SaveChangesAsync(cancellationToken);
						}
					}
				}

				// Delete templates
				foreach (var template in request.Model.DeletedTemplates)
				{
					// Find existing
					var existing = currentTemplates
						.Where(x => x.Id == template.Sid.DecodeLong())
						.SingleOrDefault();

					if (existing != null)
					{
						db.ProgramTemplates.Remove(existing);
					}
					await db.SaveChangesAsync(cancellationToken);
				}

				await transaction.CommitAsync(cancellationToken);

				return new CommandResult(true, "Templates updated.");
			}
			catch
			{
				return new CommandResult(false, Globals.UnexpectedError);
			}
		}
	}
}