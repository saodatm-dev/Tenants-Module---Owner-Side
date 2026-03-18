using Building.Domain.Statuses;
using Building.Domain.Units;

namespace Building.Application.Units.GetById;

public sealed record GetRealEstateUnitByIdResponse(
	Guid Id,
	Guid OwnerId,
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
	IEnumerable<string>? Images,
	Status Status,
	ModerationStatus ModerationStatus,
	string? Reason);
