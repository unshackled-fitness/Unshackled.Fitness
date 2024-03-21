using System.Text.Json.Serialization;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class FormTemplateSetModel : BaseObject, IGroupedSortable, ICloneable
{
	public string ExerciseSid { get; set; } = string.Empty;
	public string ListGroupSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public int SortOrder { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int SecondsTarget { get; set; }
	public int IntensityTarget { get; set; }

	[JsonIgnore]
	public TimeSpan? TimeSeconds
	{
		get => SecondsTarget > 0 ? new(0, 0, SecondsTarget) : null;
		set
		{
			SecondsTarget = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : 0;
		}
	}

	public FormTemplateSetModel() { }

	public FormTemplateSetModel(TemplateSetModel model)
	{
		DateCreatedUtc = model.DateCreatedUtc;
		DateLastModifiedUtc = model.DateLastModifiedUtc;
		Equipment = model.Equipment;
		SetMetricType = model.SetMetricType;
		ExerciseSid = model.ExerciseSid;
		ListGroupSid = model.ListGroupSid;
		IntensityTarget = model.IntensityTarget;
		IsTrackingSplit = model.IsTrackingSplit;
		Muscles = model.Muscles;
		RepMode = model.RepMode;
		RepsTarget = model.RepsTarget;
		SecondsTarget = model.SecondsTarget;
		SetType = model.SetType;
		Sid = model.Sid;
		SortOrder = model.SortOrder;
		Title = model.Title;
	}

	public object Clone()
	{
		return new FormTemplateSetModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			Equipment = Equipment,
			SetMetricType = SetMetricType,
			ExerciseSid = ExerciseSid,
			ListGroupSid = ListGroupSid,
			IntensityTarget = IntensityTarget,
			IsTrackingSplit = IsTrackingSplit,
			Muscles = Muscles,
			RepMode = RepMode,
			RepsTarget = RepsTarget,
			SecondsTarget = SecondsTarget,
			SetType = SetType,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title,
		};
	}
}
