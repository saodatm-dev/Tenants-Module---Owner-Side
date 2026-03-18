using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.LandCategories.Update;

public sealed record UpdateLandCategoryCommand(Guid Id, List<LanguageValue> Translates) : ICommand<Guid>;
