using Building.Application.Listings.GetById;
using Building.Domain.Statuses;

namespace Building.Application.Moderations.Listings.GetById;

public sealed record GetModerationListingByIdResponse(
	Guid Id,
	Guid OwnerId,
	Status Status,
	ModerationStatus ModerationStatus,
	string? Reason,
	IEnumerable<string> Categories,
	string? Complex,
	string? Building,
	IEnumerable<GetListingByIdFloorResponse>? Floors,
	IEnumerable<short>? FloorNumbers,
	int? RoomsCount,
	float TotalArea,
	float? LivingArea,
	float? CeilingHeight,
	long? PriceForMonth,
	long? PricePerSquareMeter,
	string? Description,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? ObjectNames);
