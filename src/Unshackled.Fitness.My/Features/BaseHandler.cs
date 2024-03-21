using AutoMapper;
using Unshackled.Fitness.Core.Data;

namespace Unshackled.Fitness.My.Features;

public abstract class BaseHandler
{
	protected readonly BaseDbContext db;
	protected readonly IMapper mapper;

	public BaseHandler(BaseDbContext db, IMapper mapper)
	{
		this.db = db;
		this.mapper = mapper;
	}
}
