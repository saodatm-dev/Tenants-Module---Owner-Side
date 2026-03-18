using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.WishlistItems.Create;

internal sealed class CreateWishlistItemCommandValidator : AbstractValidator<CreateWishlistItemCommand>
{
	public CreateWishlistItemCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.WishlistId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateWishlistItemCommand.WishlistId)).Description);

		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateWishlistItemCommand.RealEstateId)).Description);
	}
}
