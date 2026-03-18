using Core.Application.Resources;
using FluentValidation;

namespace Common.Application.Districts.Remove;

internal sealed class RemoveDistrictCommandValidator : AbstractValidator<RemoveDistrictCommand>
{
	public RemoveDistrictCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveDistrictCommand.Id)).Description);
}
