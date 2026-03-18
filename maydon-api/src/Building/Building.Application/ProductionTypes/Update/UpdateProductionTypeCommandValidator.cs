using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.ProductionTypes.Update;

internal sealed class UpdateProductionTypeCommandValidator : AbstractValidator<UpdateProductionTypeCommand>
{
	public UpdateProductionTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateProductionTypeCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateProductionTypeCommand.Translates)).Description);
	}
}
