using FluentValidation;
using Unshackled.Fitness.Core;
using Unshackled.Fitness.Core.Enums;

namespace Unshackled.Fitness.My.Client.Features.Exercises.Models;

public class FormExerciseModel : BaseObject
{
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public IEnumerable<MuscleTypes> Muscles { get; set; } = new List<MuscleTypes>();
	public IEnumerable<EquipmentTypes> Equipment { get; set; } = new List<EquipmentTypes>();
	public WorkoutSetTypes DefaultSetType { get; set; } = WorkoutSetTypes.Standard;
	public SetMetricTypes DefaultSetMetricType { get; set; } = SetMetricTypes.WeightReps;
	public bool IsTrackingSplit { get; set; }

	public class Validator : AbstractValidator<FormExerciseModel>
	{
		public Validator()
		{
			RuleFor(p => p.Title)
				.NotEmpty().WithMessage("Title is required.")
				.MaximumLength(255).WithMessage("Title must not exceed 255 characters.");

			RuleFor(p => p.Muscles)
				.Must(p => p != null && p.Any()).WithMessage("At least one muscle must be selected.");

			RuleFor(p => p.Equipment)
				.Must(p => p != null && p.Any()).WithMessage("At least one equipment item must be selected.");
		}
	}
}
