using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.RentalPurposes.Update;

internal sealed class UpdateRentalPurposeCommandValidator : AbstractValidator<UpdateRentalPurposeCommand>
{
	public UpdateRentalPurposeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRentalPurposeCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRentalPurposeCommand.Translates)).Description);
	}
}
