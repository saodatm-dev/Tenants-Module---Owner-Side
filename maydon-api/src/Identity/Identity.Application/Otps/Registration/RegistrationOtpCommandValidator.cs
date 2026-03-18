using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Otps.Registration;

internal sealed class RegistrationOtpCommandValidator : AbstractValidator<RegistrationOtpCommand>
{
	public RegistrationOtpCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);
	}
}
