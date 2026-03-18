using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.ProductionTypes.Create;

public sealed record CreateProductionTypeCommand(List<LanguageValue> Translates) : ICommand<Guid>;
