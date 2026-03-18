namespace Common.Application.Districts.Get;

public sealed record GetDistrictsResponse(
	Guid Id,
	string Name,
	string RegionName,
	short Order);
