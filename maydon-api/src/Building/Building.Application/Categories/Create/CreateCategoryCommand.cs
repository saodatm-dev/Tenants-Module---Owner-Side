using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Categories.Create;

public sealed record CreateCategoryCommand(
	BuildingType BuildingType,
	string IconUrl,
	List<LanguageValue> Translates) : ICommand<Guid>;
