using FluentValidation;
using Unshackled.Fitness.Core.Enums;
using Unshackled.Fitness.Core.Models;

namespace Unshackled.Fitness.My.Client.Features.WorkoutTemplates.Models;

public class FormTemplateTaskModel : BaseObject, ISortable
{
	public WorkoutTaskTypes Type { get; set; } = WorkoutTaskTypes.PreWorkout;
	public string Text { get; set; } = string.Empty;
	public int SortOrder { get; set; }

	public class Validator : BaseValidator<FormTemplateTaskModel>
	{
		public Validator()
		{
			RuleFor(x => x.Text)
				.NotEmpty().WithMessage("Item text is required.")
				.MaximumLength(255).WithMessage("Item text must not exceed 255 characters.");
		}
	}

}
