using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class FormWorkoutSetGroupModel : BaseObject, ISortableGroupForm, ICloneable
{
	public string WorkoutSid { get; set; } = string.Empty;
	public int SortOrder { get; set; }
	public string Title { get; set; } = string.Empty;
	public bool IsNew { get; set; }

	public object Clone()
	{
		return new FormWorkoutSetGroupModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			IsNew = IsNew,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title,
			WorkoutSid = WorkoutSid
		};
	}
}
