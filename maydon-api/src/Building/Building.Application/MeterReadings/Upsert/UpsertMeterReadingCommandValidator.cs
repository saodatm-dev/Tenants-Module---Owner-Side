using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.MeterReadings.Upsert;

internal sealed class UpsertMeterReadingCommandValidator : AbstractValidator<UpsertMeterReadingCommand>
{
	public UpsertMeterReadingCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.MeterId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpsertMeterReadingCommand.MeterId)).Description);



		RuleFor(item => item.PreviousValue)
			.Must(item => item is not null ? item >= 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpsertMeterReadingCommand.PreviousValue)).Description);

		RuleFor(item => item.Value)
			.Must(item => item >= 0)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpsertMeterReadingCommand.Value)).Description);

		RuleFor(item => item.Note)
			.MaximumLength(1000)
			.When(item => !string.IsNullOrWhiteSpace(item.Note))
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpsertMeterReadingCommand.Note)).Description);
	}
}
