using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.MeterTariffs.Update;

internal sealed class UpdateMeterTariffCommandValidator : AbstractValidator<UpdateMeterTariffCommand>
{
	public UpdateMeterTariffCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.Id)).Description);

		RuleFor(item => item.MeterTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.MeterTypeId)).Description);

		RuleFor(item => item.ValidFrom)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.ValidFrom)).Description);

		RuleFor(item => item.ValidTo)
			.Must(item => item is not null ? item.Value > DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.ValidTo)).Description);

		RuleFor(item => item.Price)
			.NotEmpty()
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterTariffCommand.Price)).Description);
	}
}
