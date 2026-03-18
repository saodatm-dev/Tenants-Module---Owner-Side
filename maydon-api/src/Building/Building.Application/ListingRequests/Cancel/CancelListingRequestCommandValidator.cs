using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Cancel;

internal sealed class CancelListingRequestCommandValidator : AbstractValidator<CancelListingRequestCommand>
{
	public CancelListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CancelListingRequestCommand.Id)).Description);
	}
}
