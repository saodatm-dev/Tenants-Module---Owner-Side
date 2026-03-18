using Building.Domain.Buildings;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Categories.Update;

public sealed record UpdateCategoryCommand(
	Guid Id,
	BuildingType BuildingType,
	string IconUrl,
	List<LanguageValue> Translates) : ICommand<Guid>;
