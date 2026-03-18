using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Renovations.Update;

public sealed record UpdateRenovationCommand(Guid Id, List<LanguageValue> Translates) : ICommand<Guid>;
