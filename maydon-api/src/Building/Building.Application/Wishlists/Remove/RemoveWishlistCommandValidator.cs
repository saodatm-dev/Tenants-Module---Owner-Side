using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Wishlists.Remove;

internal sealed class RemoveWishlistCommandValidator : AbstractValidator<RemoveWishlistCommand>
{
	public RemoveWishlistCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.ListingId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveWishlistCommand.ListingId)).Description);
}
