using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstateUnits.Accept;

internal sealed class AcceptModerationRealEstateUnitCommandValidator : AbstractValidator<AcceptModerationRealEstateUnitCommand>
{
	public AcceptModerationRealEstateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(AcceptModerationRealEstateUnitCommand.Id)).Description);
	}
}
