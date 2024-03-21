using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unshackled.Fitness.Core.Data;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;

namespace Unshackled.Fitness.My.Features.Metrics.Actions;

public class ListMetrics
{
	public class Query : IRequest<MetricListModel>
	{
		public long MemberId { get; private set; }

		public Query(long memberId)
		{
			MemberId = memberId;
		}
	}

	public class Handler : BaseHandler, IRequestHandler<Query, MetricListModel>
	{
		public Handler(BaseDbContext db, IMapper mapper) : base(db, mapper) { }

		public async Task<MetricListModel> Handle(Query request, CancellationToken cancellationToken)
		{
			MetricListModel model = new();
			
			model.Groups = await mapper.ProjectTo<FormMetricDefinitionGroupModel>(db.MetricDefinitionGroups
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();

			model.Metrics = await mapper.ProjectTo<FormMetricDefinitionModel>(db.MetricDefinitions
				.Where(x => x.MemberId == request.MemberId)
				.OrderBy(x => x.SortOrder))
				.ToListAsync();

			return model;
		}
	}
}