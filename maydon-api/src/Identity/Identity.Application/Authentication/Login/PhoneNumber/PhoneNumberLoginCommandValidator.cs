using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Authentication.Login.PhoneNumber;

internal sealed class PhoneNumberLoginCommandValidator : AbstractValidator<PhoneNumberLoginCommand>
{
	public PhoneNumberLoginCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);

		RuleFor(item => item.Password)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.PasswordIsEmpty(nameof(PhoneNumberLoginCommand.Password)).Description);
	}
}
