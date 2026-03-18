namespace Building.Domain.Units;


public sealed record UnitRequest(
	Guid? FloorId,
	Guid? RoomId,
	short? FloorNumber,
	string? RoomNumber,
	Guid? RenovationId,
	float TotalArea,
	float? CeilingHeight = null,
	string? Plan = null,
	IEnumerable<UnitCoordinate>? Coordinates = null,
	IEnumerable<string>? Images = null);
