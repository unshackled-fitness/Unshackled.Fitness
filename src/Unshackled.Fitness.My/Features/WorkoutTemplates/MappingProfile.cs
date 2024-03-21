using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.WorkoutTemplates;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<WorkoutTemplateEntity, TemplateListItem>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<WorkoutTemplateEntity, TemplateModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<WorkoutTemplateSetEntity, TemplateSetModel>()
			.ForMember(d => d.Equipment, m => m.MapFrom(s => s.Exercise != null ? EnumUtils.FromJoinedIntString<EquipmentTypes>(s.Exercise.Equipment) : new()))
			.ForMember(d => d.ExerciseSid, m => m.MapFrom(s => s.ExerciseId.Encode()))
			.ForMember(d => d.IsTrackingSplit, m => m.MapFrom(s => s.Exercise != null ? s.Exercise.IsTrackingSplit : false))
			.ForMember(d => d.Muscles, m => m.MapFrom(s => s.Exercise != null ? EnumUtils.FromJoinedIntString<MuscleTypes>(s.Exercise.Muscles) : new()))
			.ForMember(d => d.Title, m => m.MapFrom(s => s.Exercise != null ? s.Exercise.Title : string.Empty))
			.ForMember(d => d.ListGroupSid, m => m.MapFrom(s => s.ListGroupId.Encode()))
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<WorkoutTemplateSetGroupEntity, TemplateSetGroupModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.TemplateSid, m => m.MapFrom(s => s.WorkoutTemplateId.Encode()));
		CreateMap<WorkoutTemplateTaskEntity, TemplateTaskModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}