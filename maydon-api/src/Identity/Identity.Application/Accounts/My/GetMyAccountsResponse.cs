using Identity.Domain.Accounts;

namespace Identity.Application.Accounts.My;

public sealed record GetMyAccountsResponse(
	string Photo,
	string? FirstName,
	string? LastName,
	string? MiddleName,
	string? CompanyName,
	AccountType AccountType,
	bool isCurrent,
	string Key);
