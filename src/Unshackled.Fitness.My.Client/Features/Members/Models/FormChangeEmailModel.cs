using FluentValidation;

namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class FormChangeEmailModel
{
	public string? NewEmail { get; set; }
	public string BaseUrl { get; set; } = string.Empty;

	public class Validator : AbstractValidator<FormChangeEmailModel>
	{
		public Validator()
		{
			RuleFor(x => x.NewEmail)
				.NotEmpty().WithMessage("Required")
				.MaximumLength(255).WithMessage("Email address must not exceed 255 characters.")
				.EmailAddress().WithMessage("Must be a valid email address.");
		}
	}
}
