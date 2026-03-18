namespace Building.Application.Meters.Get;

public sealed record GetMetersResponse(
	Guid Id,
	Guid MeterTypeId,
	string MeterName,
	string SerialNumber,
	DateOnly? InstallationDate,
	DateOnly? VerificationDate,
	DateOnly? NextVerificationDate,
	decimal InitialReading);
