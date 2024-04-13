using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class ProgramModel : BaseMemberObject
{
	public string Title { get; set; } = string.Empty;
	public ProgramTypes ProgramType { get; set; }
	public string? Description { get; set; }
	public int LengthWeeks { get; set; }
	public DateTime? DateStarted { get; set; }
	public int NextTemplateIndex { get; set; }
	public string? ActiveWorkoutSid { get; set; }

	public List<ProgramTemplateModel> Templates { get; set; } = new();

	public ProgramTemplateModel? StartingTemplate()
	{
		if (!DateStarted.HasValue)
			return null;

		if (!Templates.Any())
			return null;

		if (ProgramType == ProgramTypes.FixedRepeating)
			return null;

		return Templates.First();
	}
}
