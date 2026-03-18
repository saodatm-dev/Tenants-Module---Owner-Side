using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Units.Remove;

internal sealed class RemoveUnitCommandValidator : AbstractValidator<RemoveUnitCommand>
{
	public RemoveUnitCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(x => x.Id)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.IsRequired(nameof(RemoveUnitCommand.Id)).Description);
	}
}
