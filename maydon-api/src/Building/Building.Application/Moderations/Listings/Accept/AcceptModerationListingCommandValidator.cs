using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.Listings.Accept;

internal sealed class AcceptModerationListingCommandValidator : AbstractValidator<AcceptModerationListingCommand>
{
	public AcceptModerationListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(AcceptModerationListingCommand.Id)).Description);
	}
}
