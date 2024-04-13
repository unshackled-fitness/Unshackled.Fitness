using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class SearchLibraryModel : SearchModel
{
	public string? Title { get; set; }
	public MuscleTypes MuscleType { get; set; } = MuscleTypes.Any;
	public EquipmentTypes EquipmentType { get; set; } = EquipmentTypes.Any;
}
