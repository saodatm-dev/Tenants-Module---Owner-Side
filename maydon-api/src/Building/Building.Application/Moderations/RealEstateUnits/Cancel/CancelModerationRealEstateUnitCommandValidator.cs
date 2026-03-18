using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstateUnits.Cancel;

internal sealed class CancelModerationRealEstateUnitCommandValidator : AbstractValidator<CancelModerationRealEstateUnitCommand>
{
	public CancelModerationRealEstateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CancelModerationRealEstateUnitCommand.Id)).Description);
	}
}
