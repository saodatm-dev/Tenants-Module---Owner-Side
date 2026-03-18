using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.AmenityCategories.Remove;

internal sealed class RemoveAmenityCategoryCommandValidator : AbstractValidator<RemoveAmenityCategoryCommand>
{
	public RemoveAmenityCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveAmenityCategoryCommand.Id)).Description);
}
