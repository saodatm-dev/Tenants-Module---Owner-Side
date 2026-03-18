namespace Identity.Application.Companies.Get;

public sealed record GetCompaniesResponse(
	Guid Id,
	string Name,
	string? Tin,
	bool IsVerified,
	string? Photo);
