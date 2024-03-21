using FluentValidation;

namespace Unshackled.Fitness.My.Client.Features.Programs.Models;

public class FormUpdateProgramModel
{
	public string Sid { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }

	public class Validator : BaseValidator<FormUpdateProgramModel>
	{
		public Validator()
		{
			RuleFor(x => x.Title)
				.NotEmpty().WithMessage("Title is required.")
				.MaximumLength(255).WithMessage("Title must not exceed 255 characters.");
		}
	}
}
