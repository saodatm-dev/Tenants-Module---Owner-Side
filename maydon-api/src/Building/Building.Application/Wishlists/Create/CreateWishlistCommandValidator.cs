using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Wishlists.Create;

internal sealed class CreateWishlistCommandValidator : AbstractValidator<CreateWishlistCommand>
{
	public CreateWishlistCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.ListingId)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateWishlistCommand.ListingId)).Description);
	}
}
