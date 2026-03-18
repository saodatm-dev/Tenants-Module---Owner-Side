using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Floors.Remove;

internal sealed class RemoveFloorCommandValidator : AbstractValidator<RemoveFloorCommand>
{
	public RemoveFloorCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveFloorCommand.Id)).Description);
}
