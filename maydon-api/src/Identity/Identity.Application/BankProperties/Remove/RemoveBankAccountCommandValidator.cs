using Core.Application.Resources;
using FluentValidation;


namespace Identity.Application.BankProperties.Remove;

internal sealed class RemoveBankAccountCommandValidator : AbstractValidator<RemoveBankAccountCommand>
{
	public RemoveBankAccountCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveBankAccountCommand.Id)).Description);
}
