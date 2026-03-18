using Building.Domain.Buildings;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingCategories.Create;

internal sealed class CreateListingCategoryCommandValidator : AbstractValidator<CreateListingCategoryCommand>
{
	public CreateListingCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.BuildingType)
			.Must(item => BuildingType.Commercial <= item && item <= BuildingType.Living)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCategoryCommand.BuildingType)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCategoryCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateListingCategoryCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCategoryCommand.Translates)).Description);

		RuleFor(item => item.ParentId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateListingCategoryCommand.ParentId)).Description);
	}
}
