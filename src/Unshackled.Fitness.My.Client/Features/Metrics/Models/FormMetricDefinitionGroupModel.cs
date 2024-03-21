using Unshackled.Fitness.Core;

namespace Unshackled.Fitness.My.Client.Features.Metrics.Models;

public class FormMetricDefinitionGroupModel : BaseMemberObject, ISortableGroupForm, ICloneable
{
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool IsNew { get; set; }

	public object Clone()
	{
		return new FormMetricDefinitionGroupModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			IsNew = IsNew,
			MemberSid = MemberSid,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title
		};
	}
}
