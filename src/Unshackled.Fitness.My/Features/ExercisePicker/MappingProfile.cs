using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.ExercisePicker;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ExerciseEntity, ExerciseModel>()
			.ForMember(d => d.Equipment, m => m.MapFrom(s => EnumUtils.FromJoinedIntString<EquipmentTypes>(s.Equipment)))
			.ForMember(d => d.Muscles, m => m.MapFrom(s => EnumUtils.FromJoinedIntString<MuscleTypes>(s.Muscles)))
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
