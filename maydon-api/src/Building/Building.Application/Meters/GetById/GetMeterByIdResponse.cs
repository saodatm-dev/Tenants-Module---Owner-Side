namespace Building.Application.Meters.GetById;

public sealed record GetMeterByIdResponse(
	Guid RealEstateId,
	Guid MeterTypeId,
	string MeterName,
	string SerialNumber,
	DateOnly? InstallationDate,
	DateOnly? VerificationDate,
	DateOnly? NextVerificationDate,
	decimal InitialReading);
