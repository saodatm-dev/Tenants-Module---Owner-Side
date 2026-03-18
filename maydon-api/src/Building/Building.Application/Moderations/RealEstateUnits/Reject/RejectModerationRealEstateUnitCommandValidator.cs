using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstateUnits.Reject;

internal sealed class RejectModerationRealEstateUnitCommandValidator : AbstractValidator<RejectModerationRealEstateUnitCommand>
{
	public RejectModerationRealEstateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RejectModerationRealEstateUnitCommand.Id)).Description);

		RuleFor(item => item.Reason)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Reason))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(RejectModerationRealEstateUnitCommand.Reason), 500).Description);
	}
}
