using Core.Application.Abstractions.Messaging;

namespace Common.Application.Languages.Update;

public sealed record UpdateLanguageCommand(
	Guid Id,
	string Name,
	string ShortCode) : ICommand<Guid>;
