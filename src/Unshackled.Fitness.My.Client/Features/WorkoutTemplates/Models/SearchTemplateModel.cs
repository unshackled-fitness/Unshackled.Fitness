using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class SearchTemplateModel : SearchModel
{
	public string? Title { get; set; }
	public MuscleTypes MuscleType { get; set; } = MuscleTypes.Any;
}
