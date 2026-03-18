using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Amenities.Create;

internal sealed class CreateAmenityCommandValidator : AbstractValidator<CreateAmenityCommand>
{
	public CreateAmenityCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.AmenityCategoryId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateAmenityCommand.AmenityCategoryId)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateAmenityCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateAmenityCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateAmenityCommand.Translates)).Description);
	}
}
