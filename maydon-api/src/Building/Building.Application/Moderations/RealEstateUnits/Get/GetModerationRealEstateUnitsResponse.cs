using Building.Domain.Statuses;

namespace Building.Application.Moderations.Get;

public sealed record GetModerationRealEstateUnitsResponse(
	Guid Id,
	Guid OwnerId,
	string? RoomNumber,
	short? FloorNumber,
	float? TotalArea,
	float? CeilingHeight,
	Status Status,
	ModerationStatus ModerationStatus,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? ObjectNames);
