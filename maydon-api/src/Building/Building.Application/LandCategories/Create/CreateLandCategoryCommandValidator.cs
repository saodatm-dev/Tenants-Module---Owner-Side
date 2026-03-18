using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.LandCategories.Create;

internal sealed class CreateLandCategoryCommandValidator : AbstractValidator<CreateLandCategoryCommand>
{
	public CreateLandCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateLandCategoryCommand.Translates)).Description);
}
