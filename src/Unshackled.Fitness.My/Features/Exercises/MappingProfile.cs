using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Utils;
using Unshackled.Fitness.My.Client.Features.Exercises.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Exercises;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ExerciseEntity, ExerciseModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.Equipment, m => m.MapFrom(s => EnumUtils.FromJoinedIntString<EquipmentTypes>(s.Equipment)))
			.ForMember(d => d.Muscles, m => m.MapFrom(s => EnumUtils.FromJoinedIntString<MuscleTypes>(s.Muscles)));
		CreateMap<ExerciseEntity, MergeExerciseModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
		CreateMap<WorkoutSetEntity, RecordListModel>()
			.ForMember(d => d.WorkoutSetSid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.WorkoutSid, m => m.MapFrom(s => s.WorkoutId.Encode()))
			.ForMember(d => d.DateWorkoutUtc, m => m.MapFrom(s => s.Workout != null ? s.Workout.DateCompletedUtc : s.DateRecordedUtc));
		CreateMap<WorkoutSetEntity, ResultListModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.DateWorkoutUtc, m => m.MapFrom(s => s.Workout != null ? s.Workout.DateCompletedUtc : s.DateRecordedUtc))
			.ForMember(d => d.ListGroupSid, m => m.MapFrom(s => s.WorkoutId.Encode()));
	}
}
