using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Login.EImzoMobile;

internal sealed class EImzoMobileLoginCommandValidator : AbstractValidator<EImzoMobileLoginCommand>
{
	public EImzoMobileLoginCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.DocumentId)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoMobileLoginCommand.DocumentId)).Description);
	}
}
