using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.Listings.Block;

internal sealed class BlockModerationListingCommandValidator : AbstractValidator<BlockModerationListingCommand>
{
	public BlockModerationListingCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(BlockModerationListingCommand.Id)).Description);

		RuleFor(item => item.Reason)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Reason))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(BlockModerationListingCommand.Reason), 500).Description);
	}
}
