using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Accounts.Activate;

public sealed record ActivateAccountCommand(Guid UserId) : ICommand<Guid>;
