using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingRequests.Create;

internal sealed class CreateListingRequestCommandValidator : AbstractValidator<CreateListingRequestCommand>
{
	public CreateListingRequestCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.ListingId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingRequestCommand.ListingId)).Description);

		RuleFor(item => item.Content)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingRequestCommand.Content)).Description)
			.MaximumLength(1000)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateListingRequestCommand.Content), 2000).Description);
	}
}
