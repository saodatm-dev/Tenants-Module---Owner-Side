using Building.Domain.Floors;
using Building.Domain.Rooms;
using Building.Domain.Units;
using Core.Application.Abstractions.Messaging;

namespace Building.Application.RealEstates.Create;

public sealed record CreateRealEstateCommand(
	Guid RealEstateTypeId,
	Guid? RenovationId,
	Guid? LandCategoryId,
	Guid? ProductionTypeId,
	float TotalArea,
	string? CadastralNumber = null,
	string? Number = null,
	Guid? BuildingId = null,
	Guid? FloorId = null,
	string? BuildingNumber = null,
	short? FloorNumber = null,
	float? LivingArea = null,
	float? CeilingHeight = null,
	short? TotalFloors = null,
	short? AboveFloors = null,
	short? BelowFloors = null,
	short? RoomsCount = null,
	IEnumerable<UnitRequest>? Units = null,
	IEnumerable<RoomValue>? Rooms = null,
	Guid? RegionId = null,
	Guid? DistrictId = null,
	double? Latitude = null,
	double? Longitude = null,
	string? Address = null,
	string? plan = null,
	IEnumerable<string>? Images = null,
	IEnumerable<Guid>? AmenityIds = null) : ICommand<Guid>;
