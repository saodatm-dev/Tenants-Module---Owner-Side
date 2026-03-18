namespace Building.Application.MeterReadings.GetById;

public sealed record GetMeterReadingByIdResponse(
	Guid RealEstateId,
	Guid MeterId,
	string MeterName,
	string SerialNumber,
	DateOnly ReadingDate,
	decimal Value,
	decimal PreviousValue,
	decimal Consumption,
	bool IsManual);
