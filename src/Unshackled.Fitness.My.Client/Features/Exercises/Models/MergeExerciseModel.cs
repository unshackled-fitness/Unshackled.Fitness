using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class MergeExerciseModel : BaseObject
{
	public string Title { get; set; } = string.Empty;
	public Guid? MatchId { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
}