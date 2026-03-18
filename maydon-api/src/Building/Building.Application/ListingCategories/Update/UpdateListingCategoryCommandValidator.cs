using Building.Domain.Buildings;
using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ListingCategories.Update;

internal sealed class UpdateListingCategoryCommandValidator : AbstractValidator<UpdateListingCategoryCommand>
{
	public UpdateListingCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCategoryCommand.Id)).Description);

		RuleFor(item => item.BuildingType)
			.Must(item => BuildingType.Commercial <= item && item <= BuildingType.Living)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCategoryCommand.BuildingType)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCategoryCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateListingCategoryCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCategoryCommand.Translates)).Description);

		RuleFor(item => item.ParentId)
			.Must(item => item != null ? item != Guid.Empty : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateListingCategoryCommand.ParentId)).Description);
	}
}
