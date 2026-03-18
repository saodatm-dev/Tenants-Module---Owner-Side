using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Districts.MoveUp;

internal sealed class MoveUpDistrictCommandValidator : AbstractValidator<MoveUpDistrictCommand>
{
	public MoveUpDistrictCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(MoveUpDistrictCommand.Id)).Description);
}
