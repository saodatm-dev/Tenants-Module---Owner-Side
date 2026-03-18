using Core.Application.Resources;
using Core.Domain.Providers;
using FluentValidation;

namespace Building.Application.Meters.Create;

internal sealed class CreateMeterCommandValidator : AbstractValidator<CreateMeterCommand>
{
	public CreateMeterCommandValidator(
		ISharedViewLocalizer sharedViewLocalizer,
		IDateTimeProvider dateTimeProvider)
	{
		RuleFor(item => item.RealEstateId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.RealEstateId)).Description);

		RuleFor(item => item.MeterTypeId)
			.NotEmpty()
			.NotEqual(Guid.Empty)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.MeterTypeId)).Description);

		RuleFor(item => item.SerialNumber)
			.NotEmpty()
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.SerialNumber)).Description);

		RuleFor(item => item.InstallationDate)
			.Must(item => item is not null ? item < DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.InstallationDate)).Description);

		RuleFor(item => item.VerificationDate)
			.Must(item => item is not null ? item < DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.VerificationDate)).Description);

		RuleFor(item => item.NextVerificationDate)
			.Must(item => item is not null ? item > DateOnly.FromDateTime(dateTimeProvider.UtcNow) : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.NextVerificationDate)).Description);

		RuleFor(item => item.InitialReading)
			.Must(item => item is not null ? item > 0 : true)
			.WithMessage(sharedViewLocalizer.InvalidValue(nameof(CreateMeterCommand.InitialReading)).Description);
	}
}
