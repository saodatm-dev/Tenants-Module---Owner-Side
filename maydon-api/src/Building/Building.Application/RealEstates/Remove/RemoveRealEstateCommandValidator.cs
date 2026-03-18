using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstates.Remove;

internal sealed class RemoveRealEstateCommandValidator : AbstractValidator<RemoveRealEstateCommand>
{
	public RemoveRealEstateCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRealEstateCommand.Id)).Description);
}
