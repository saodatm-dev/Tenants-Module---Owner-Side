using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;

namespace Identity.Application.Otps.RestorePassword;

internal sealed class RestorePasswordOtpCommandValidator : AbstractValidator<RestorePasswordOtpCommand>
{
	public RestorePasswordOtpCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);
	}
}
