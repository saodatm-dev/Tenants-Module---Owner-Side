using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Amenities.Remove;

internal sealed class RemoveAmenityCommandValidator : AbstractValidator<RemoveAmenityCommand>
{
	public RemoveAmenityCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveAmenityCommand.Id)).Description);
}
