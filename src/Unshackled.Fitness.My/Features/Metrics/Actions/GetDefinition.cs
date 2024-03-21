using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class GetDefinition
{
	public class Query : IRequest<FormMetricDefinitionModel>
	{
		public long MemberId { get; private set; }
		public long Id { get; private set; }

		public Query(long memberId, long id)
		{
			MemberId = memberId;
			Id = id;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, FormMetricDefinitionModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<FormMetricDefinitionModel> Handle(Query request, CancellationToken cancellationToken)
		{
			return await mapper.ProjectTo<FormMetricDefinitionModel>(db.MetricDefinitions
				.Where(x => x.MemberId == request.MemberId && x.Id == request.Id))
				.SingleOrDefaultAsync() ?? new();
		}
	}
}