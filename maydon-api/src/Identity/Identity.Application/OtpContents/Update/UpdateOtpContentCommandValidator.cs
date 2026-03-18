using Core.Application.Resources;
using FluentValidation;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.Update;

internal sealed class UpdateOtpContentCommandValidator : AbstractValidator<UpdateOtpContentCommand>
{
	public UpdateOtpContentCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.OtpType)
			.Must(item => OtpType.Registration <= item && item <= OtpType.InviteByUserId)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateOtpContentCommand.OtpType)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateOtpContentCommand.Translates)).Description);
	}
}
