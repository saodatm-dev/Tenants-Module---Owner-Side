using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstates.Block;

internal sealed class BlockModerationRealEstateCommandValidator : AbstractValidator<BlockModerationRealEstateCommand>
{
	public BlockModerationRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(BlockModerationRealEstateCommand.Id)).Description);
	}
}
