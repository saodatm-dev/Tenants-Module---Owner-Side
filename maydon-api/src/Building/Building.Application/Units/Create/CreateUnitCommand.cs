using Building.Domain.Units;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Units.Create;

public sealed record CreateUnitCommand(
	Guid? RealEstateId,
	Guid? RealEstateTypeId,
	Guid? FloorId,
	Guid? RoomId,
	Guid? RenovationId,
	short? FloorNumber,
	string? RoomNumber,
	float TotalArea,
	float? CeilingHeight = null,
	string? Plan = null,
	IEnumerable<UnitCoordinate>? Coordinates = null,
	IEnumerable<string>? Images = null) : ICommand<Guid>;
