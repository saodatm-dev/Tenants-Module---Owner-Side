using Core.Domain.Languages;

namespace Building.Application.LandCategories.GetById;

public sealed record GetLandCategoryByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
