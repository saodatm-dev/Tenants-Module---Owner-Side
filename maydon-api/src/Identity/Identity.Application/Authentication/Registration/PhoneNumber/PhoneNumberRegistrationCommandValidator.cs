using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.Registration.PhoneNumber;

internal sealed class PhoneNumberRegistrationCommandValidator : AbstractValidator<PhoneNumberRegistrationCommand>
{
	public PhoneNumberRegistrationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);

		RuleFor(item => item.Code)
			.NotEmpty()
			.Length(6)
			.WithMessage(sharedViewLocalizer.IsEmpty(nameof(PhoneNumberRegistrationCommand.Code)).Description);
	}
}
