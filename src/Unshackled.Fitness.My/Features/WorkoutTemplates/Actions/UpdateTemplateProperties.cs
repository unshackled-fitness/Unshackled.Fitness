using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates.Actions;

public class UpdateTemplateProperties
{
	public class Command : IRequest<CommandResult<TemplateModel>>
	{
		public long MemberId { get; private set; }
		public FormTemplateModel Model { get; private set; }

		public Command(long memberId, FormTemplateModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<TemplateModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<TemplateModel>> Handle(Command request, CancellationToken cancellationToken)
		{
			long templateId = request.Model.Sid.DecodeLong();

			var template = await db.WorkoutTemplates
				.Where(x => x.Id == templateId && x.MemberId == request.MemberId)
				.SingleOrDefaultAsync();

			if (template == null)
				return new CommandResult<TemplateModel>(false, "Invalid template.");

			// Update template
			template.Description = request.Model.Description?.Trim();
			template.Title = request.Model.Title.Trim();
			await db.SaveChangesAsync(cancellationToken);

			return new CommandResult<TemplateModel>(true, "Template updated.", mapper.Map<TemplateModel>(template));
		}
	}
}