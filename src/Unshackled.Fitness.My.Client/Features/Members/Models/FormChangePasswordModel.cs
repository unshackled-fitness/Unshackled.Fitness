using FluentValidation;

namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class FormChangePasswordModel
{
	public string OldPassword { get; set; } = "";

	public string NewPassword { get; set; } = "";

	public string ConfirmPassword { get; set; } = "";

	public class Validator : AbstractValidator<FormChangePasswordModel>
	{
		public Validator()
		{
			RuleFor(x => x.OldPassword)
				.NotEmpty().WithMessage("Required");

			RuleFor(x => x.NewPassword)
				.NotEmpty().WithMessage("Required")
				.MinimumLength(6).WithMessage("Password must be at least 6 characters.")
				.MaximumLength(100).WithMessage("Password must not exceed 100 characters.");

			RuleFor(x => x.ConfirmPassword)
				.NotEmpty().WithMessage("Required")
				.Equal(x => x.NewPassword).WithMessage("The new password and confirmation password do not match.");
		}
	}
}
