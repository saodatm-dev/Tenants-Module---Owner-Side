using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ProductionTypes.Create;

internal sealed class CreateProductionTypeCommandValidator : AbstractValidator<CreateProductionTypeCommand>
{
	public CreateProductionTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateProductionTypeCommand.Translates)).Description);
}
