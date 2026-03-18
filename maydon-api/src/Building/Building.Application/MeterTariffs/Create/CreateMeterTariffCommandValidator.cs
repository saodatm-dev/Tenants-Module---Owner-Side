using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.MeterTariffs.Create;

internal sealed class CreateMeterTariffCommandValidator : AbstractValidator<CreateMeterTariffCommand>
{
	public CreateMeterTariffCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.MeterTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTariffCommand.MeterTypeId)).Description);

		RuleFor(item => item.ValidFrom)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTariffCommand.ValidFrom)).Description);

		RuleFor(item => item.ValidTo)
			.Must(item => item is not null ? item.Value > DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTariffCommand.ValidTo)).Description);

		RuleFor(item => item.Price)
			.NotEmpty()
			.GreaterThan(0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterTariffCommand.Price)).Description);
	}
}
