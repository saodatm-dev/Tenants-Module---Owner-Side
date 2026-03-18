using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.Meters.Update;

internal sealed class UpdateMeterCommandValidator : AbstractValidator<UpdateMeterCommand>
{
	public UpdateMeterCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.Id)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.Id)).Description);

		RuleFor(item => item.MeterTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.MeterTypeId)).Description);

		RuleFor(item => item.SerialNumber)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.SerialNumber)).Description);

		RuleFor(item => item.InstallationDate)
			.Must(item => item is not null ? item < DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.InstallationDate)).Description);

		RuleFor(item => item.VerificationDate)
			.Must(item => item is not null ? item < DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.VerificationDate)).Description);

		RuleFor(item => item.NextVerificationDate)
			.Must(item => item is not null ? item > DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.NextVerificationDate)).Description);

		RuleFor(item => item.InitialReading)
			.Must(item => item is not null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(UpdateMeterCommand.InitialReading)).Description);
	}
}
