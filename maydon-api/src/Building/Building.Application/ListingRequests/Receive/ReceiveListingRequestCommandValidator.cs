using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Receive;

internal sealed class ReceiveListingRequestCommandValidator : AbstractValidator<ReceiveListingRequestCommand>
{
	public ReceiveListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(ReceiveListingRequestCommand.Id)).Description);
	}
}
