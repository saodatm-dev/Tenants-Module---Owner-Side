using Core.Domain.Languages;

namespace Building.Application.AmenityCategories.GetById;

public sealed record GetAmenityCategoryByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
