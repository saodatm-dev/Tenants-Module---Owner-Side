using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ProductionTypes.Remove;

internal sealed class RemoveProductionTypeCommandValidator : AbstractValidator<RemoveProductionTypeCommand>
{
	public RemoveProductionTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveProductionTypeCommand.Id)).Description);
	}
}
