using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Currencies.Remove;

internal sealed class RemoveCurrencyCommandValidator : AbstractValidator<RemoveCurrencyCommand>
{
	public RemoveCurrencyCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveCurrencyCommand.Id)).Description);
}
