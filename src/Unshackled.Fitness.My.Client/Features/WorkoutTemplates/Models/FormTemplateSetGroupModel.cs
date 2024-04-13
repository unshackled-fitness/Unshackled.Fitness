using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class FormTemplateSetGroupModel : ISortableGroupForm, ICloneable
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public bool IsNew { get; set; }

	public object Clone()
	{
		return new FormTemplateSetGroupModel
		{
			IsNew = IsNew,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title,
		};
	}
}
