using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class SetTheme
{
	public class Command : IRequest<CommandResult<Member>>
	{
		public long Id { get; private set; }
		public Themes Theme { get; private set; }

		public Command(long id, Themes theme)
		{
			Id = id;
			Theme = theme;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Command, CommandResult<Member>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<CommandResult<Member>> Handle(Command request, CancellationToken cancellationToken)
		{
			var member = await db.Members
				.Where(x => x.Id == request.Id)
				.SingleOrDefaultAsync(cancellationToken);

			if (member == null)
				return new CommandResult<Member>(false, "Invalid member.");

			var settings = await db.GetMemberSettings(request.Id);
			settings.AppTheme = request.Theme;
			await db.SaveMemberSettings(request.Id, settings);

			var m = await db.GetMember(member);

			return new CommandResult<Member>(true, "Theme updated.", m);
		}
	}
}
