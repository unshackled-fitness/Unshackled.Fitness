using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.ExercisePicker.Models;

public class SearchExerciseModel : SearchModel
{
	public string? Title { get; set; }
	public MuscleTypes MuscleType { get; set; } = MuscleTypes.Any;
	public EquipmentTypes EquipmentType { get; set; } = EquipmentTypes.Any;
}
