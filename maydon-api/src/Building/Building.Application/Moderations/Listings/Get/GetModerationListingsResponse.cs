using Building.Domain.Statuses;

namespace Building.Application.Moderations.Listings.Get;

public sealed record GetModerationListingsResponse(
	Guid Id,
	Guid OwnerId,
	string? ComplexName,
	string? BuildingNumber,
	string? Address,
	IEnumerable<string> Categories,
	float TotalArea,
	int? RoomsCount,
	long? PriceForMonth,
	long? PricePerSquareMeter,
	string? Description,
	Status Status,
	ModerationStatus ModerationStatus,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? ObjectNames);
