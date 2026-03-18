using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Bindings.OneId;

internal sealed class OneIdBindingCommandValidator : AbstractValidator<OneIdBindingCommand>
{
	public OneIdBindingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Code)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(OneIdBindingCommand.Code)).Description);

		RuleFor(item => item.UpdateUserData)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(OneIdBindingCommand.UpdateUserData)).Description);
	}
}
