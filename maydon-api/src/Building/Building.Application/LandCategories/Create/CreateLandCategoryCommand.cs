using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.LandCategories.Create;

public sealed record CreateLandCategoryCommand(List<LanguageValue> Translates) : ICommand<Guid>;
