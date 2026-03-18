using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Bindings.EImzo;

public sealed record EImzoBindingCommand(string Pkcs7, bool UpdateUserData = false) : ICommand<bool>;
