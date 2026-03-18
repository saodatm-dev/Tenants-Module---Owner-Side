using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.AmenityCategories.Create;

public sealed record CreateAmenityCategoryCommand(List<LanguageValue> Translates) : ICommand<Guid>;
