using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Complexes.Remove;

internal sealed class RemoveComplexCommandValidator : AbstractValidator<RemoveComplexCommand>
{
	public RemoveComplexCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveComplexCommand.Id)).Description);
}
