using FluentValidation;

namespace Unshackled.Fitness.My.Client.Features.Members.Models;

public class FormAuthenticatorModel
{
	public string Code { get; set; } = "";

	public class Validator : AbstractValidator<FormAuthenticatorModel>
	{
		public Validator()
		{
			RuleFor(x => x.Code)
				.NotEmpty().WithMessage("Required")
				.MinimumLength(6).WithMessage("Code must be at least 6 characters.")
				.MaximumLength(7).WithMessage("Code must not exceed 7 characters.");
		}
	}
}
