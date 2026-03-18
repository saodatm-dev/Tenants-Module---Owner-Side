using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.AmenityCategories.Update;

public sealed record UpdateAmenityCategoryCommand(
	Guid Id,
	List<LanguageValue> Translates) : ICommand<Guid>;
