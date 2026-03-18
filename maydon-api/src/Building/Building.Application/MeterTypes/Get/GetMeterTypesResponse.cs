namespace Building.Application.MeterTypes.Get;

public sealed record GetMeterTypesResponse(
	Guid Id,
	string Name,
	string Description,
	string? Icon);
