using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Configuration;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Members.Actions;

public class GetMemberByEmail
{
	public class Query : IRequest<Member?>
	{
		public string Email { get; private set; }

		public Query(string email)
		{
			Email = email;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, Member?>
	{
		private readonly SiteConfiguration siteConfig;

		public Handler(BaseDbContext db, IMapper mapper, SiteConfiguration siteConfig) : base(db, mapper) 
		{
			this.siteConfig = siteConfig;
		}

		public async Task<Member?> Handle(Query request, CancellationToken cancellationToken)
		{
			var memberEntity = await db.Members
				.AsNoTracking()
				.Where(s => s.Email == request.Email.ToLower())
				.SingleOrDefaultAsync(cancellationToken);

			if (memberEntity == null)
			{
				memberEntity = await db.AddMember(request.Email, siteConfig);
				if (memberEntity == null)
					return null;
			}

			return await db.GetMember(memberEntity);
		}
	}
}
