using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.ForgotPassword.PhoneNumber;

internal sealed class PhoneNumberForgotPasswordCommandValidator : AbstractValidator<PhoneNumberForgotPasswordCommand>
{
	public PhoneNumberForgotPasswordCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);

		RuleFor(item => item.Code)
			.NotEmpty()
			.Length(6)
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(PhoneNumberForgotPasswordCommand.Code)).Description);
	}
}
