using AutoMapper;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;
using Unshackled.Fitness.Core.Data.Entities;

namespace Unshackled.Fitness.My.Features.Dashboard;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<WorkoutEntity, WorkoutModel>();
	}
}
