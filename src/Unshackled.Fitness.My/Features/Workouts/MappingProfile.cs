using AutoMapper;
using Unshackled.Fitness.Core.Data.Entities;
using Unshackled.Fitness.My.Client.Features.Workouts.Models;
using Unshackled.Fitness.My.Extensions;

namespace Unshackled.Fitness.My.Features.Workouts;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<ExerciseEntity, ExerciseNoteModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));

		CreateMap<WorkoutEntity, WorkoutListModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));

		CreateMap<WorkoutEntity, FormWorkoutModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));

		CreateMap<WorkoutSetGroupEntity, FormWorkoutSetGroupModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.WorkoutSid, m => m.MapFrom(s => s.WorkoutId.Encode()));

		// Need this for EF core even though we don't call it directly.
		CreateMap<WorkoutSetEntity, FormWorkoutSetModel>();

		CreateMap<WorkoutSetEntity, CompletedSetModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()))
			.ForMember(d => d.DateWorkoutUtc, m => m.MapFrom(s => s.Workout != null ? s.Workout.DateCompletedUtc : s.DateRecordedUtc))
			.ForMember(d => d.ExerciseSid, m => m.MapFrom(s => s.ExerciseId.Encode()))
			.ForMember(d => d.ListGroupSid, m => m.MapFrom(s => s.WorkoutId.Encode()));

		CreateMap<WorkoutTaskEntity, FormWorkoutTaskModel>()
			.ForMember(d => d.Sid, m => m.MapFrom(s => s.Id.Encode()));
	}
}
