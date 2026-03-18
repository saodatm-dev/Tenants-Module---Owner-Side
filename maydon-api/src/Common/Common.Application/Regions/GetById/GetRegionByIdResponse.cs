using Core.Domain.Languages;

namespace Common.Application.Regions.GetById;

public sealed record GetRegionByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Translates);
