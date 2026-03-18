using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RentalPurposes.Remove;

internal sealed class RemoveRentalPurposeCommandValidator : AbstractValidator<RemoveRentalPurposeCommand>
{
	public RemoveRentalPurposeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRentalPurposeCommand.Id)).Description);
}
