using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Accounts.Deactivate;

public sealed record DeactivateAccountCommand(Guid UserId) : ICommand<Guid>;
