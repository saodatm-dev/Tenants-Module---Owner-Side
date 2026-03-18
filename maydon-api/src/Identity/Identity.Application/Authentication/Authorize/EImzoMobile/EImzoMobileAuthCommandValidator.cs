using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Authorize.EImzoMobile;

internal sealed class EImzoMobileAuthCommandValidator : AbstractValidator<EImzoMobileAuthCommand>
{
	public EImzoMobileAuthCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.DocumentId)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoMobileAuthCommand.DocumentId)).Description);
	}
}
