using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Dashboard.Models;

namespace Unshackled.Fitness.My.Features.Dashboard;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<WorkoutEntity, WorkoutModel>();
	}
}
