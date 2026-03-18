using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstates.Cancel;

internal sealed class CancelModerationRealEstateCommandValidator : AbstractValidator<CancelModerationRealEstateCommand>
{
	public CancelModerationRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CancelModerationRealEstateCommand.Id)).Description);
	}
}
