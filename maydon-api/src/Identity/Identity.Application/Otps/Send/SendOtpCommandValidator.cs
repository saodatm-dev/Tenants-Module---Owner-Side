using Core.Application.Resources;
using FluentValidation;
using Identity.Application.Core.Validators;


namespace Identity.Application.Otps.Send;

internal sealed class SendOtpCommandValidator : AbstractValidator<SendOtpCommand>
{
	public SendOtpCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.PhoneNumber)
			.PhoneNumber(sharedViewLocalizer);

		RuleFor(item => item.OtpType)
			.IsInEnum()
			.Must(item => item == Domain.Otps.OtpType.Registration || item == Domain.Otps.OtpType.RestorePassword)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(SendOtpCommand.OtpType)).Description);
	}
}
