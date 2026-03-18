using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Bindings.OneId;

public sealed record OneIdBindingCommand(Guid Code, bool UpdateUserData = false) : ICommand<bool>;
