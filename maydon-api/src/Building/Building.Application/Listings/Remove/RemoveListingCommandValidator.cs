using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Listings.Remove;

internal sealed class RemoveListingCommandValidator : AbstractValidator<RemoveListingCommand>
{
	public RemoveListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveListingCommand.Id)).Description);
}
