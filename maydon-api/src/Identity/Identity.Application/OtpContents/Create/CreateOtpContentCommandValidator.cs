using Core.Application.Resources;
using FluentValidation;
using Identity.Domain.Otps;

namespace Identity.Application.OtpContents.Create;

internal sealed class CreateOtpContentCommandValidator : AbstractValidator<CreateOtpContentCommand>
{
	public CreateOtpContentCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.OtpType)
			.Must(item => OtpType.Registration <= item && item <= OtpType.InviteByUserId)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateOtpContentCommand.OtpType)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateOtpContentCommand.Translates)).Description);
	}
}
