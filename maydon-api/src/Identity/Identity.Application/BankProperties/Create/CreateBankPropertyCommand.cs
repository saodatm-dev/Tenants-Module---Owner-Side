using Core.Application.Abstractions.Messaging;

namespace Identity.Application.BankProperties.Create;

public sealed record CreateBankPropertyCommand(
	Guid BankId,
	string BankName,
	string BankMfo,
	string AccountNumber,
	bool IsMain,
	bool IsPublic) : ICommand<Guid>;
