using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Banks.Remove;

internal sealed class RemoveBankCommandValidator : AbstractValidator<RemoveBankCommand>
{
	public RemoveBankCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveBankCommand.Id)).Description);
}
