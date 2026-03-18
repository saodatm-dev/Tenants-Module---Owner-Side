using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RentalPurposes.Create;

internal sealed class CreateRentalPurposeCommandValidator : AbstractValidator<CreateRentalPurposeCommand>
{
	public CreateRentalPurposeCommandValidator(ISharedViewLocalizer sharedViewLocalizer) =>
		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateRentalPurposeCommand.Translates)).Description);
}
