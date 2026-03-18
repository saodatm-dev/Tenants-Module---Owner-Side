using Core.Application.Abstractions.Messaging;

namespace Identity.Application.Users.UpdateLogo;

public sealed record UpdateUserLogoCommand(string? ObjectName) : ICommand;
