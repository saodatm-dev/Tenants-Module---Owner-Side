using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Authentication.Registration.EImzo;

internal sealed class EImzoRegistrationCommandValidator : AbstractValidator<EImzoRegistrationCommand>
{
	public EImzoRegistrationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Pkcs7)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoRegistrationCommand.Pkcs7)).Description);
	}
}
