using Building.Domain.Statuses;

namespace Building.Application.RealEstates.GetMy;

public sealed record GetMyRealEstatesResponse(
	Guid Id,
	Guid OwnerId,
	ModerationStatus ModerationStatus,
	string? Building,
	short? Floor,
	string? Number,
	int? RoomsCount,
	float? Area,
	float? LivingArea,
	float? CeilingHeight,
	string? Region,
	string? District,
	double? Latitude,
	double? Longitude,
	string? Address,
	Status Status,
	string? Reason = null);
