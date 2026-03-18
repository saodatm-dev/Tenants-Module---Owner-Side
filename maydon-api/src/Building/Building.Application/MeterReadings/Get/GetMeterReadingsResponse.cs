namespace Building.Application.MeterReadings.Get;

public sealed record GetMeterReadingsResponse(
	Guid MeterId,
	string MeterName,
	string SerialNumber,
	DateOnly ReadingDate,
	decimal Value,
	decimal PreviousValue,
	decimal Consumption,
	bool IsManual);
