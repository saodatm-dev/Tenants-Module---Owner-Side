using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.Accounts.Change;

internal sealed class ChangeAccountCommandValidator : AbstractValidator<ChangeAccountCommand>
{
	public ChangeAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Key)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(ChangeAccountCommand.Key)).Description);
	}
}
