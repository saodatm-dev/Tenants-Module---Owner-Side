using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RealEstateTypes.Remove;

internal sealed class RemoveRealEstateTypeCommandValidator : AbstractValidator<RemoveRealEstateTypeCommand>
{
	public RemoveRealEstateTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveRealEstateTypeCommand.Id)).Description);
}
