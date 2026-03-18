using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.LandCategories.Remove;

internal sealed class RemoveLandCategoryCommandValidator : AbstractValidator<RemoveLandCategoryCommand>
{
	public RemoveLandCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(RemoveLandCategoryCommand.Id)).Description);

}
