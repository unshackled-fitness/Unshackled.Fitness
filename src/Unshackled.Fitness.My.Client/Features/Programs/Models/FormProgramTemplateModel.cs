using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class FormProgramTemplateModel : BaseMemberObject, ICloneable, ISortable
{
	public string ProgramSid { get; set; } = string.Empty;
	public string WorkoutTemplateSid { get; set; } = string.Empty;
	public string WorkoutTemplateName { get; set; } = string.Empty;
	public int WeekNumber { get; set; }
	public int DayNumber { get; set; }
	public int SortOrder { get; set; }
	public bool IsNew { get; set; }

	public object Clone()
	{
		return new FormProgramTemplateModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			DayNumber = DayNumber,
			IsNew = IsNew,
			MemberSid = MemberSid,
			ProgramSid = ProgramSid,
			Sid = Sid,
			SortOrder = SortOrder,
			WeekNumber = WeekNumber,
			WorkoutTemplateName = WorkoutTemplateName,
			WorkoutTemplateSid = WorkoutTemplateSid
		};
	}
}
