using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.MeterTypes.Create;

internal sealed class CreateMeterTypeCommandValidator : AbstractValidator<CreateMeterTypeCommand>
{
	public CreateMeterTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Names)
			.NotEmpty()
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTypeCommand.Names)).Description);

		RuleFor(item => item.Description)
			.NotEmpty()
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTypeCommand.Description)).Description);

		RuleFor(item => item.Icon)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Icon))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(CreateMeterTypeCommand.Icon), 500).Description);
	}
}
