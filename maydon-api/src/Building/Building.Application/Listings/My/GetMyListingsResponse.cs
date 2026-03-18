using Building.Domain.Statuses;

namespace Building.Application.Listings.My;

public sealed record GetMyListingsResponse(
    Guid Id,
    Guid OwnerId,
    string? Title,
    ModerationStatus ModerationStatus,
    IEnumerable<string>? Categories,
    string? Building,
    List<short>? FloorNumbers,
    int? RoomsCount,
    float TotalArea,
    float? LivingArea,
    float? CeilingHeight,
    IEnumerable<string>? ObjectNames,
    string? Region,
    string? District,
    double? Latitude,
    double? Longitude,
    string? Address,
    Status Status,
    string Reason);
