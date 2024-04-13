using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class ProgramTemplateModel : BaseMemberObject
{
	public string ProgramSid { get; set; } = string.Empty;
	public string WorkoutTemplateSid { get; set; } = string.Empty;
	public string WorkoutTemplateName { get; set; } = string.Empty;
	public int WeekNumber { get; set; }
	public int DayNumber { get; set; }
	public int SortOrder { get; set; }
}
