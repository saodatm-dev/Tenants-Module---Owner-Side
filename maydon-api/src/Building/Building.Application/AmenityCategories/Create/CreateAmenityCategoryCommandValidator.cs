using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.AmenityCategories.Create;

internal sealed class CreateAmenityCategoryCommandValidator : AbstractValidator<CreateAmenityCategoryCommand>
{
	public CreateAmenityCategoryCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateAmenityCategoryCommand.Translates)).Description);
	}
}
