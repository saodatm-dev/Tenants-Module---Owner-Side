using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Remove;

internal sealed class RemoveListingRequestCommandValidator : AbstractValidator<RemoveListingRequestCommand>
{
	public RemoveListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveListingRequestCommand.Id)).Description);
	}
}
