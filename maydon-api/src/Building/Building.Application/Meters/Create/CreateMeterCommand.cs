using Core.Application.Abstractions.Messaging;

namespace Building.Application.Meters.Create;

public sealed record CreateMeterCommand(
	Guid RealEstateId,
	Guid MeterTypeId,
	string SerialNumber,
	DateOnly? InstallationDate,
	DateOnly? VerificationDate,
	DateOnly? NextVerificationDate,
	decimal? InitialReading) : ICommand<Guid>;
