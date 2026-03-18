namespace Common.Application.Regions.Get;

public sealed record GetRegionsResponse(
	Guid Id,
	string Name,
	short Order);
