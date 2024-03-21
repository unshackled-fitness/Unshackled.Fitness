using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.Core.Data.Extensions;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs.Actions;

public class SearchPrograms
{
	public class Query : IRequest<SearchResult<ProgramListModel>>
	{
		public long MemberId { get; private set; }
		public SearchProgramModel Model { get; private set; }

		public Query(long memberId, SearchProgramModel model)
		{
			MemberId = memberId;
			Model = model;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, SearchResult<ProgramListModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<SearchResult<ProgramListModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			var result = new SearchResult<ProgramListModel>();
			var query = db.Programs
				.AsNoTracking()
				.Where(x => x.MemberId == request.MemberId);

			if (request.Model.ProgramType != ProgramTypes.Any)
			{
				query = query.Where(x => x.ProgramType == request.Model.ProgramType);
			}

			if (!string.IsNullOrEmpty(request.Model.Title))
			{
				query = query.Where(x => x.Title.StartsWith(request.Model.Title));
			}

			result.Total = await query.CountAsync(cancellationToken);

			if (request.Model.Sorts.Any())
			{
				query = query.AddSorts(request.Model.Sorts);
			}
			else
			{
				query = query.OrderBy(x => x.Title);
			}

			result.Data = await query
				.Select(x => new ProgramListModel
				{
					DateCreatedUtc = x.DateCreatedUtc,
					DateLastModifiedUtc = x.DateLastModifiedUtc,
					DateStarted = x.DateStarted,
					LengthWeeks = x.LengthWeeks,
					MemberSid = x.MemberId.Encode(),
					ProgramType = x.ProgramType,
					Sid = x.Id.Encode(),
					Title = x.Title,
					Workouts = db.ProgramTemplates.Where(y => y.ProgramId == x.Id).Count(),
				})
				.Skip(request.Model.Skip)
				.Take(request.Model.PageSize)
				.ToListAsync(cancellationToken);

			return result;
		}
	}
}
