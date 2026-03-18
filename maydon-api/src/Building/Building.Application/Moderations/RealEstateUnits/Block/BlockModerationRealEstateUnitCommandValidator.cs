using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Moderations.RealEstateUnits.Block;

internal sealed class BlockModerationRealEstateUnitCommandValidator : AbstractValidator<BlockModerationRealEstateUnitCommand>
{
	public BlockModerationRealEstateUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(BlockModerationRealEstateUnitCommand.Id)).Description);
	}
}
