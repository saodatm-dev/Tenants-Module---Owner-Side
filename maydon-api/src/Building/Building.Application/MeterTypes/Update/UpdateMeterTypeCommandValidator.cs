using Core.Application.Resources;
using FluentValidation;

namespace Building.Application.MeterTypes.Update;

internal sealed class UpdateMeterTypeCommandValidator : AbstractValidator<UpdateMeterTypeCommand>
{
	public UpdateMeterTypeCommandValidator(ISharedViewLocalizer sharedViewLocalizer)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTypeCommand.Id)).Description);

		RuleFor(item => item.Names)
			.NotEmpty()
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTypeCommand.Names)).Description);

		RuleFor(item => item.Description)
			.NotEmpty()
			.Must(item => item.All(n => !string.IsNullOrWhiteSpace(n.Value)))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTypeCommand.Description)).Description);

		RuleFor(item => item.Icon)
			.MaximumLength(500)
			.When(item => !string.IsNullOrEmpty(item.Icon))
			.WithMessage(sharedViewLocalizer.MaximumValueHasToBe(nameof(UpdateMeterTypeCommand.Icon), 500).Description);
	}
}
