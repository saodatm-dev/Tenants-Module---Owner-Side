using Core.Domain.Languages;

namespace Building.Application.Renovations.GetById;

public sealed record GetRenovationByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
