using Building.Domain.Statuses;

namespace Building.Application.Listings.Get;

public sealed record GetListingsResponse(
	Guid Id,
	Guid OwnerId,
	string? Title,
	List<Guid> CategoryIds,
	IEnumerable<string> Categories,
	string? Image,
	string? Complex,
	string? Building,
	float? TotalArea,
	int? FloorsCount,
	string Description,
	long? PriceForMonth,
	long? PricePerSquareMeter,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
	Status Status);

