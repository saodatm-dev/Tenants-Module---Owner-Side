using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Update;

internal sealed class UpdateListingRequestCommandValidator : AbstractValidator<UpdateListingRequestCommand>
{
	public UpdateListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingRequestCommand.Id)).Description);

		RuleFor(item => item.Content)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingRequestCommand.Content)).Description)
			.MaximumLength(2000)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateListingRequestCommand.Content), 2000).Description);
	}
}
