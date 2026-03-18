using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Districts.MoveDown;

internal sealed class MoveDownDistrictCommandValidator : AbstractValidator<MoveDownDistrictCommand>
{
	public MoveDownDistrictCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(MoveDownDistrictCommand.Id)).Description);
}
