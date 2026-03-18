using Core.Application.Resources;
using FluentValidation;

namespace Identity.Application.Authentication.Login.EImzo;

internal sealed class EImzoLoginCommandValidator : AbstractValidator<EImzoLoginCommand>
{
	public EImzoLoginCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Pkcs7)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoLoginCommand.Pkcs7)).Description);
	}
}
