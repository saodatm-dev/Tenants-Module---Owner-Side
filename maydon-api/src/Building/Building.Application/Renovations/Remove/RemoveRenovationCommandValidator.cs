using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Renovations.Remove;

internal sealed class RemoveRenovationCommandValidator : AbstractValidator<RemoveRenovationCommand>
{
	public RemoveRenovationCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRenovationCommand.Id)).Description);

}
