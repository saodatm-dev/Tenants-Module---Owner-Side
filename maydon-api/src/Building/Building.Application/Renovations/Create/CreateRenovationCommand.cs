using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Renovations.Create;

public sealed record CreateRenovationCommand(List<LanguageValue> Translates) : ICommand<Guid>;
