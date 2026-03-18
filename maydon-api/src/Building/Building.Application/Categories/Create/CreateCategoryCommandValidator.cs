using Building.Domain.Buildings;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Categories.Create;

internal sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
	public CreateCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.BuildingType)
			.Must(item => BuildingType.Commercial <= item && item <= BuildingType.Living)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateCategoryCommand.BuildingType)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateCategoryCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateCategoryCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateCategoryCommand.Translates)).Description);
	}
}
