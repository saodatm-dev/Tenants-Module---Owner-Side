using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.Registration.PhoneNumberConfirm;

internal sealed class PhoneNumberRegistrationConfirmCommandValidator : AbstractValidator<PhoneNumberRegistrationConfirmCommand>
{
	public PhoneNumberRegistrationConfirmCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Key)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(PhoneNumberRegistrationConfirmCommand.Key)).Description);

		RuleFor(item => item.Password)
			.Password(sharedViewLocalizer);
	}
}
