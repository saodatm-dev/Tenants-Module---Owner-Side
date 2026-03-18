using Core.Domain.ValueObjects;

namespace Building.Application.Leases.Get;

public sealed record GetLeasesResponse(
	Guid Id,
	Guid OwnerId,
	Guid ClientId,
	DateOnly StartDate,
	DateOnly? EndDate,
	short PaymentDay,
	string Status,
	int ItemsCount,
	string? FirstPropertyAddress,
	Money TotalMonthlyRent);
