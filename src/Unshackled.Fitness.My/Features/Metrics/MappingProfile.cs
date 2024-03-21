using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Metrics.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Metrics;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<MetricDefinitionEntity, FormMetricDefinitionModel>()
			.ForMember(d => d.ListGroupSid, m => m.MapFrom(s => s.ListGroupId.Encode()))
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<MetricDefinitionGroupEntity, FormMetricDefinitionGroupModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
