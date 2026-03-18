namespace Identity.Application.BankProperties.Get;

public sealed record GetBankPropertiesResponse(
	Guid Id,
	string BankName,
	string BankMfo,
	string AccountNumber,
	bool IsMain,
	bool IsPublic);
