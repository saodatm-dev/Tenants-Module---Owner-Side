using Building.Domain.Statuses;

namespace Building.Application.Moderations.RealEstates.Get;

public sealed record GetModerationRealEstatesResponse(
	Guid Id,
	Guid OwnerId,
	string? Number,
	string? BuildingNumber,
	short? FloorNumber,
	string? Address,
	string RealEstateType,
	float? TotalArea,
	int? RoomsCount,
	Status Status,
	ModerationStatus ModerationStatus,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? ObjectNames);
