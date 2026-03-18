using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Renovations.Create;

internal sealed class CreateRenovationCommandValidator : AbstractValidator<CreateRenovationCommand>
{
	public CreateRenovationCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRenovationCommand.Translates)).Description);
}
