using Identity.Domain.Accounts;

namespace Identity.Application.Accounts.Get;

public sealed record GetAccountsResponse(
	string Photo,
	string Name,
	AccountType AccountType,
	string Key);
