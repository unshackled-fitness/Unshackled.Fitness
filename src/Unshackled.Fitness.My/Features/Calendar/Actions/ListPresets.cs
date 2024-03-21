using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Calendar.Models;

namespace Unshackled.Fitness.My.Features.Calendar.Actions;

public class ListPresets
{
	public class Query : IRequest<List<PresetModel>>
	{
		public long MemberId { get; private set; }

		public Query(long memberId)
		{
			MemberId = memberId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, List<PresetModel>>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<List<PresetModel>> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<PresetModel>(db.MetricPresets
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.Title))
				.ToListAsync();
		}
	}
}