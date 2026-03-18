using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.LandCategories.Update;

internal sealed class UpdateLandCategoryCommandValidator : AbstractValidator<UpdateLandCategoryCommand>
{
	public UpdateLandCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLandCategoryCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateLandCategoryCommand.Translates)).Description);
	}

}
