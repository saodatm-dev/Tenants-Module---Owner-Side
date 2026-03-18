using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.Listings.Reject;

internal sealed class RejectModerationListingCommandValidator : AbstractValidator<RejectModerationListingCommand>
{
	public RejectModerationListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RejectModerationListingCommand.Id)).Description);

		RuleFor(item => item.Reason)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Reason))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(RejectModerationListingCommand.Reason), 500).Description);
	}
}
