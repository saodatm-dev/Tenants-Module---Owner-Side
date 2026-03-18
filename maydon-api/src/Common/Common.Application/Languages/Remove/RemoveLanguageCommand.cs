using Core.Application.Abstractions.Messaging;

namespace Common.Application.Languages.Remove;

public sealed record RemoveLanguageCommand(Guid Id) : ICommand;
