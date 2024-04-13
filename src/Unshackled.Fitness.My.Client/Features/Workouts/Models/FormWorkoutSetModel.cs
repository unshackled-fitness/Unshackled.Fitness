using System.Text.Json.Serialization;
using FluentValidation;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;
using Unshackled.Fitness.Core.Utils;

namespace Unshackled.Fitness.My.Client.Features.Workouts.Models;

public class FormWorkoutSetModel : BaseObject, IGroupedSortable, ICloneable
{
	public string WorkoutSid { get; set; } = string.Empty;
	public string ExerciseSid { get; set; } = string.Empty;
	public string ListGroupSid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public bool IsTrackingSplit { get; set; }
	public WorkoutSetTypes SetType { get; set; }
	public SetMetricTypes SetMetricType { get; set; }
	public List<MuscleTypes> Muscles { get; set; } = new();
	public List<EquipmentTypes> Equipment { get; set; } = new();
	public string? ExerciseNotes { get; set; }
	public int SortOrder { get; set; }
	public RepModes RepMode { get; set; }
	public int RepsTarget { get; set; }
	public int IntensityTarget { get; set; }
	public int? Reps { get; set; }
	public int? RepsLeft { get; set; }
	public int? RepsRight { get; set; }
	public int Seconds { get; set; }
	public int SecondsLeft { get; set; }
	public int SecondsRight { get; set; }
	public int SecondsTarget { get; set; }
	public decimal? Weight { get; set; }
	public WeightUnits WeightUnit { get; set; } = WeightUnits.lb;
	public DateTime? DateRecordedUtc { get; set; }
	public bool IsBestSetBySeconds { get; set; }
	public bool IsBestSetByVolume { get; set; }
	public bool IsBestSetByWeight { get; set; }
	public bool IsRecordTargetVolume { get; set; }
	public bool IsRecordTargetWeight { get; set; }
	public bool IsRecordSeconds { get; set; }
	public bool IsRecordSecondsAtWeight { get; set; }
	public bool IsRecordVolume { get; set; }
	public bool IsRecordWeight { get; set; }

	[JsonIgnore]
	public bool IsEditing { get; set; } = false; 
	
	[JsonIgnore]
	public bool IsExpanded { get; set; } = false;

	[JsonIgnore]
	public bool IsSaving { get; set; } = false;

	[JsonIgnore]
	public bool HasTarget => RepsTarget > 0;

	[JsonIgnore]
	public TimeSpan? TargetTimeSeconds
	{
		get => SecondsTarget > 0 ? new(0, 0, SecondsTarget) : null;
		set
		{
			SecondsTarget = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : 0;
		}
	}

	[JsonIgnore]
	public TimeSpan? TimeSeconds
	{
		get => Seconds > 0 ? new(0, 0, Seconds) : null;
		set
		{
			Seconds = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : 0;
		}
	}

	[JsonIgnore]
	public TimeSpan? TimeSecondsLeft
	{
		get => SecondsLeft > 0 ? new(0, 0, SecondsLeft) : null;
		set
		{
			SecondsLeft = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : 0;
		}
	}

	[JsonIgnore]
	public TimeSpan? TimeSecondsRight
	{
		get => SecondsRight > 0 ? new(0, 0, SecondsRight) : null;
		set
		{
			SecondsRight = value.HasValue ? (int)Math.Round(value.Value.TotalSeconds, 0) : 0;
		}
	}

	[JsonIgnore]
	public decimal Volume => IsTrackingSplit
		? Calculator.Volume(Weight, RepsLeft, RepsRight)
		: Calculator.Volume(Weight, Reps);

	[JsonIgnore]
	public Validator ModelValidator { get; set; } = new();

	[JsonIgnore]
	public bool RepsRequired => SetMetricType == SetMetricTypes.WeightReps || SetMetricType == SetMetricTypes.RepsOnly;

	[JsonIgnore]
	public bool WeightRequired => SetMetricType == SetMetricTypes.WeightReps || SetMetricType == SetMetricTypes.WeightTime;

	public object Clone()
	{
		return new FormWorkoutSetModel
		{
			DateCreatedUtc = DateCreatedUtc,
			DateLastModifiedUtc = DateLastModifiedUtc,
			DateRecordedUtc = DateRecordedUtc,
			Equipment = Equipment,
			ExerciseNotes = ExerciseNotes,
			SetMetricType = SetMetricType,
			ExerciseSid = ExerciseSid,
			ListGroupSid = ListGroupSid,
			IntensityTarget = IntensityTarget,
			IsExpanded = IsExpanded,
			IsEditing = IsEditing,
			IsSaving = IsSaving,
			IsTrackingSplit = IsTrackingSplit,
			Muscles = Muscles,
			RepMode = RepMode,
			Reps = Reps,
			RepsLeft = RepsLeft,
			RepsRight = RepsRight,
			RepsTarget = RepsTarget,
			Seconds = Seconds,
			SecondsLeft = SecondsLeft,
			SecondsRight = SecondsRight,
			SecondsTarget = SecondsTarget,
			SetType = SetType,
			Sid = Sid,
			SortOrder = SortOrder,
			Title = Title,
			Weight = Weight,
			WeightUnit = WeightUnit,
			WorkoutSid = WorkoutSid
		};
	}

	public class Validator : BaseValidator<FormWorkoutSetModel>
	{
		public Validator()
		{
			RuleFor(x => x.Weight)
				.NotNull()
				.When(x => x.WeightRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(0)
				.When(x => x.WeightRequired == true)
				.WithMessage("Weight must be a positive number.")
				.LessThanOrEqualTo(AppGlobals.MaxSetWeight)
				.When(x => x.WeightRequired == true)
				.WithMessage($"Weight must be less than {Math.Ceiling(AppGlobals.MaxSetWeight)}.");

			RuleFor(x => x.Reps)
				.NotNull()
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage("Reps must be greater than zero.")
				.LessThanOrEqualTo(AppGlobals.MaxSetReps)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == true)
				.WithMessage($"Reps must be less than or equal to {AppGlobals.MaxSetReps}.");

			RuleFor(x => x.RepsLeft)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true)
				.WithMessage("Required")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsRight == 0)
				.WithMessage("Left reps must be greater than zero.")
				.LessThanOrEqualTo(AppGlobals.MaxSetReps)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsRight == 0)
				.WithMessage($"Left reps must be less than or equal to {AppGlobals.MaxSetReps}.");

			RuleFor(x => x.RepsRight)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true)
				.WithMessage("Required.")
				.GreaterThanOrEqualTo(1)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsLeft == 0)
				.WithMessage("Right reps must be greater than zero.")
				.LessThanOrEqualTo(AppGlobals.MaxSetReps)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == true && x.RepsLeft == 0)
				.WithMessage($"Right reps must be less than or equal to {AppGlobals.MaxSetReps}.");

			RuleFor(x => x.TimeSeconds)
				.NotNull()
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == false)
				.WithMessage("Required")
				.Must(x => x != null && x.Value.TotalSeconds > 0)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == false)
				.WithMessage("Time must be greater than zero.")
				.Must(x => x != null && x.Value.TotalSeconds <= 86400)
				.When(x => x.IsTrackingSplit == false && x.RepsRequired == false)
				.WithMessage("Time must be less than 24 hours.");

			RuleFor(x => x.TimeSecondsLeft)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false)
				.WithMessage("Required")
				.Must(x => x != null && x.Value.TotalSeconds > 0)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false && x.SecondsRight == 0)
				.WithMessage("Time must be greater than zero.")
				.Must(x => x != null && x.Value.TotalSeconds <= 86400)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false && x.SecondsRight == 0)
				.WithMessage("Left Time must be less than 24 hours.");

			RuleFor(x => x.TimeSecondsRight)
				.NotNull()
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false)
				.WithMessage("Required")
				.Must(x => x != null && x.Value.TotalSeconds > 0)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false && x.SecondsLeft == 0)
				.WithMessage("Time must be greater than zero.")
				.Must(x => x != null && x.Value.TotalSeconds <= 86400)
				.When(x => x.IsTrackingSplit == true && x.RepsRequired == false && x.SecondsLeft == 0)
				.WithMessage("Right Time must be less than 24 hours.");
		}
	}
}
