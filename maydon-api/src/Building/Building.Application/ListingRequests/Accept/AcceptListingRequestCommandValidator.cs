using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Accept;

internal sealed class AcceptListingRequestCommandValidator : AbstractValidator<AcceptListingRequestCommand>
{
	public AcceptListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(AcceptListingRequestCommand.Id)).Description);
	}
}
