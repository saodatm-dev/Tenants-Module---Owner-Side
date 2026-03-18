using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.Listings.Cancel;

internal sealed class CancelModerationListingCommandValidator : AbstractValidator<CancelModerationListingCommand>
{
	public CancelModerationListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CancelModerationListingCommand.Id)).Description);
	}
}
