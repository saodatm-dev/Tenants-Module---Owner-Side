using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingCategories.Remove;

internal sealed class RemoveListingCategoryCommandValidator : AbstractValidator<RemoveListingCategoryCommand>
{
	public RemoveListingCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveListingCategoryCommand.Id)).Description);
}
