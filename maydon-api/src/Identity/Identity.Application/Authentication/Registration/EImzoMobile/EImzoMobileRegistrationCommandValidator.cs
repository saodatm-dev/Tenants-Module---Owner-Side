using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Registration.EImzoMobile;

internal sealed class EImzoMobileRegistrationCommandValidator : AbstractValidator<EImzoMobileRegistrationCommand>
{
	public EImzoMobileRegistrationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.DocumentId)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoMobileRegistrationCommand.DocumentId)).Description);
	}
}
