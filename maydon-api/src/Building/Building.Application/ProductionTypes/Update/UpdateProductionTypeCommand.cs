using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.ProductionTypes.Update;

public sealed record UpdateProductionTypeCommand(Guid Id, List<LanguageValue> Translates) : ICommand<Guid>;
