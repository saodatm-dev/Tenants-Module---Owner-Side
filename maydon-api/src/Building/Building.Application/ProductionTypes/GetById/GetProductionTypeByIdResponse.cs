using Core.Domain.Languages;

namespace Building.Application.ProductionTypes.GetById;

public sealed record GetProductionTypeByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
