using Building.Domain.Statuses;

namespace Building.Application.Moderations.RealEstates.GetById;

public sealed record GetModerationRealEstateByIdResponse(
	Guid Id,
	Guid OwnerId,
	Status Status,
	ModerationStatus ModerationStatus,
	string? Reason,
	string Type,
	string? BuildingName,
	short? Floor,
	string? Number,
	int? RoomsCount,
	float? TotalArea,
	float? LivingArea,
	float? CeilingHeight,
	string? CadastralNumber,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
	string? Plan,
	DateTimeOffset CreatedAt,
	IEnumerable<string>? ObjectNames);
