using Core.Domain.ValueObjects;

namespace Building.Application.Leases.GetById;

public sealed record GetLeaseByIdResponse(
	Guid Id,
	Guid OwnerId,
	Guid? AgentId,
	Guid ClientId,
	DateOnly StartDate,
	DateOnly? EndDate,
	short PaymentDay,
	string? ContractNumber,
	string Status,
	IReadOnlyList<LeaseItemResponse> Items);

public sealed record LeaseItemResponse(
	Guid Id,
	Guid ListingId,
	Guid? RealEstateId,
	Guid? RealEstateUnitId,
	string Category,
	string? Building,
	short? FloorNumber,
	int? RoomsCount,
	float? TotalArea,
	float? LivingArea,
	float? CeilingHeight,
	string? Address,
	Money MonthlyRent,
	Money DepositAmount,
	bool IsMetersIncluded,
	IEnumerable<string>? Images = null);
