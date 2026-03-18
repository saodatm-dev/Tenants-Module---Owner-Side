using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Reject;

internal sealed class RejectListingRequestCommandValidator : AbstractValidator<RejectListingRequestCommand>
{
	public RejectListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RejectListingRequestCommand.Id)).Description);

		RuleFor(item => item.Reason)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Reason))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(RejectListingRequestCommand.Reason), 500).Description);
	}
}
