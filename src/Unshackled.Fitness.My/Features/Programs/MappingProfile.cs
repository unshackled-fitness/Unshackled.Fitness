using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Programs.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Programs;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ProgramEntity, ProgramListModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode())); 
		CreateMap<ProgramEntity, ProgramModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<ProgramTemplateEntity, ProgramTemplateModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.ProgramSid, m => m.MapFrom(s => s.ProgramId.Encode()))
			.ForMember(d => d.WorkoutTemplateSid, m => m.MapFrom(s => s.WorkoutTemplateId.Encode()))
			.ForMember(d => d.WorkoutTemplateName, m => m.MapFrom(s => s.WorkoutTemplate != null ? s.WorkoutTemplate.Title : string.Empty));
		CreateMap<WorkoutTemplateEntity, TemplateListModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
