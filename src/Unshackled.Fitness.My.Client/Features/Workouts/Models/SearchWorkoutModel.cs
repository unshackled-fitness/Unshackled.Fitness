using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class SearchWorkoutModel : SearchModel
{
	public DateTime? DateStart { get; set; }
	public DateTime? DateEnd { get; set; }
	public string? Title { get; set; }
	public MuscleTypes MuscleType { get; set; } = MuscleTypes.Any;
}