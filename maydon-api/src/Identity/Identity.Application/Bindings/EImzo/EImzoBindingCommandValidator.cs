using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Bindings.EImzo;

internal sealed class EImzoBindingCommandValidator : AbstractValidator<EImzoBindingCommand>
{
	public EImzoBindingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Pkcs7)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoBindingCommand.Pkcs7)).Description);

		RuleFor(item => item.UpdateUserData)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(EImzoBindingCommand.UpdateUserData)).Description);
	}
}
