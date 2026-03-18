using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Amenities.Update;

internal sealed class UpdateAmenityCommandValidator : AbstractValidator<UpdateAmenityCommand>
{
	public UpdateAmenityCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateAmenityCommand.Id)).Description);

		RuleFor(item => item.AmenityCategoryId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateAmenityCommand.AmenityCategoryId)).Description);

		RuleFor(item => item.IconUrl)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateAmenityCommand.IconUrl)).Description)
			.MaximumLength(500)
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateAmenityCommand.IconUrl), 500).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(t => !string.IsNullOrWhiteSpace(t.Value) && t.LanguageId != Guid.Empty))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateAmenityCommand.Translates)).Description);
	}
}
