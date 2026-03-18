using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Regions.MoveUp;

internal sealed class MoveUpRegionCommandValidator : AbstractValidator<MoveUpRegionCommand>
{
	public MoveUpRegionCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(MoveUpRegionCommand.Id)).Description);
}
