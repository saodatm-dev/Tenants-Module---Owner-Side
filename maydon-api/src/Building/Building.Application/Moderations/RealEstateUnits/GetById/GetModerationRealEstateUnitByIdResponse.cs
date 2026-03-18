using Building.Domain.Statuses;

namespace Building.Application.Moderations.RealEstateUnits.GetById;

public sealed record GetModerationRealEstateUnitByIdResponse(
	Guid Id,
	Guid OwnerId,
	Status Status,
	ModerationStatus ModerationStatus,
	string? Reason,
	string Type,
	short? Floor,
	string? Room,
	float? TotalArea,
	float? CeilingHeight,
	string? Plan,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? Images);
