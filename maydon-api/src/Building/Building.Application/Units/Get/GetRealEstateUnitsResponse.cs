using Building.Domain.Statuses;
using Building.Domain.Units;

namespace Building.Application.Units.Get;

public sealed record GetRealEstateUnitsResponse(
	Guid Id,
	Guid? RealEstateId,
	Guid? RealEstateTypeId,
	short? FloorNumber,
	string? RoomNumber,
	float? TotalArea,
	float? CeilingHeight,
	Guid? RenovationId,
	string? RenovationName,
	string? Plan,
	IEnumerable<UnitCoordinate>? Coordinates,
	string? Reason,
	Status Status,
	ModerationStatus ModerationStatus);
