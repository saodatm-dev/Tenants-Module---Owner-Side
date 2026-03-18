using Building.Domain.Buildings;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Categories.Update;

internal sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
	public UpdateCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCategoryCommand.Id)).Description);

		RuleFor(item => item.BuildingType)
			.NotEmpty()
			.Must(item => BuildingType.Commercial <= item && item <= BuildingType.Living)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCategoryCommand.BuildingType)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCategoryCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateCategoryCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateCategoryCommand.Translates)).Description);
	}
}
