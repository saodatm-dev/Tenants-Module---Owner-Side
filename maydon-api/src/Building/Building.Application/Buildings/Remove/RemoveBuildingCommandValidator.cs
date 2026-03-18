using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Buildings.Remove;

internal sealed class RemoveBuildingCommandValidator : AbstractValidator<RemoveBuildingCommand>
{
	public RemoveBuildingCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveBuildingCommand.Id)).Description);
}
