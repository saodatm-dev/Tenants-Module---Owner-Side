using Building.Domain.Units;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.Units.Update;

public sealed record UpdateUnitCommand(
	Guid Id,
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
