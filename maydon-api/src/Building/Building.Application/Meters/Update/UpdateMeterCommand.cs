using Core.Application.Abstractions.Messaging;

namespace Building.Application.Meters.Update;

public sealed record UpdateMeterCommand(
	Guid Id,
	Guid MeterTypeId,
	string SerialNumber,
	DateOnly? InstallationDate,
	DateOnly? VerificationDate,
	DateOnly? NextVerificationDate,
	decimal? InitialReading) : ICommand<Guid>;
