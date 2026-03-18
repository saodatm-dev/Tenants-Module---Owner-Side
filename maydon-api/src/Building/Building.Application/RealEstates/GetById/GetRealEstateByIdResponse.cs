namespace Building.Application.RealEstates.GetById;

public sealed record GetRealEstateByIdResponse(
	Guid Id,
	Guid RealEstateTypeId,
	string? RealEstateTypeName,
	float? TotalArea,
	Guid? RenovationId,
	string? RenovationName,
	Guid? LandCategoryId,
	string? LandCategoryName,
	Guid? ProductionTypeId,
	string? ProductionTypeName,
	string? CadastralNumber,
	string? Number,
	Guid? BuildingId,
	string? BuildingNumber,
	IEnumerable<Guid>? FloorIds,
	short? FloorNumber,
	short? TotalFloors,
	short? AboveFloors,
	short? BelowFloors,
	Guid? RegionId,
	string? RegionName,
	Guid? DistrictId,
	string? DistrictName,
	string? Address,
	double? Latitude,
	double? Longitude,
	float? LivingArea,
	float? CeilingHeight,
	int? RoomsCount,
	string? Plan,
	IEnumerable<string>? Images,
	IEnumerable<RealEstateRoomResponse>? Rooms,
	IEnumerable<RealEstateUnitResponse>? Units,
	IEnumerable<RealEstateAmenityResponse>? Amenities,
	string Status,
	string ModerationStatus,
	DateTime CreatedAt,
	DateTime? UpdatedAt);

public sealed record RealEstateAmenityResponse(
	Guid Id,
	string Name,
	string? IconUrl,
	Guid CategoryId,
	string? CategoryName);

public sealed record RealEstateRoomResponse(
	Guid Id,
	Guid RoomTypeId,
	string? RoomTypeName,
	float? Area);

public sealed record RealEstateUnitResponse(
	Guid Id,
	Guid? RealEstateTypeId,
	string? RealEstateTypeName,
	Guid? RenovationId,
	string? RenovationName,
	float? TotalArea,
	short? FloorNumber,
	float? LivingArea,
	float? CeilingHeight,
	string? Plan,
	IEnumerable<string>? Images,
	string Status,
	string ModerationStatus);
