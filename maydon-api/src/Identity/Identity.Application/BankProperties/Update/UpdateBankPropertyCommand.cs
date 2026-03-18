using Core.Application.Abstractions.Messaging;

namespace Identity.Application.BankProperties.Update;

public sealed record UpdateBankPropertyCommand(
	Guid Id,
	Guid BankId,
	string BankName,
	string BankMfo,
	string AccountNumber,
	bool IsMain,
	bool IsPublic) : ICommand<Guid>;
