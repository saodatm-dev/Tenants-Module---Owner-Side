using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Regions.MoveDown;

internal sealed class MoveDownRegionCommandValidator : AbstractValidator<MoveDownRegionCommand>
{
	public MoveDownRegionCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(MoveDownRegionCommand.Id)).Description);
}
