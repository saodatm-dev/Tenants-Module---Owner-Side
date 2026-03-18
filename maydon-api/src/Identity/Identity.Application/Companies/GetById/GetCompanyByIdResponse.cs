namespace Identity.Application.Companies.GetById;

public sealed record GetCompanyByIdResponse(
	Guid Id,
	string Name,
	string? Tin,
	bool IsVerified,
	string? Photo);
