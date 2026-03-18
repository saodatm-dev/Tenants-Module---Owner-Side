namespace Identity.Application.BankProperties.GetById;

public sealed record GetBankPropertyByIdResponse(
	Guid Id,
	string BankName,
	string BankMfo,
	string AccountNumber,
	bool IsMain,
	bool IsPublic);
