using Core.Domain.Languages;

namespace Building.Application.Categories.GetById;

public sealed record GetCategoryByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
