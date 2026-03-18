using Core.Application.Abstractions.Messaging;

namespace Common.Application.Languages.Create;

public sealed record CreateLanguageCommand(
	string Name,
	string ShortCode) : ICommand<Guid>;
