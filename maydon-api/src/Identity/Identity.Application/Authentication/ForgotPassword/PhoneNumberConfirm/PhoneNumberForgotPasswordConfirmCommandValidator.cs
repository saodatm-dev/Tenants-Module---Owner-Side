using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.ForgotPassword.PhoneNumberConfirm;

internal sealed class PhoneNumberForgotPasswordConfirmCommandValidator : AbstractValidator<PhoneNumberForgotPasswordConfirmCommand>
{
	public PhoneNumberForgotPasswordConfirmCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Key)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(PhoneNumberForgotPasswordConfirmCommand.Key)).Description);

		RuleFor(item => item.Password)
			.Password(sharedViewLocalizer);
	}
}
