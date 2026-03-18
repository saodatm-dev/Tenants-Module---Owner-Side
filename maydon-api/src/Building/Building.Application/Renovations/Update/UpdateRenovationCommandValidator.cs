using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.Renovations.Update;

internal sealed class UpdateRenovationCommandValidator : AbstractValidator<UpdateRenovationCommand>
{
	public UpdateRenovationCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRenovationCommand.Id)).Description);

		RuleFor(item => item.Translates)
			.NotEmpty()
			.Must(item => item.All(value => value.LanguageId != Guid.Empty && !string.IsNullOrWhiteSpace(value.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateRenovationCommand.Translates)).Description);
	}

}
