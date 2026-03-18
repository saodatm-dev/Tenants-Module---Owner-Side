using Core.Domain.Languages;

namespace Common.Application.Districts.GetById;

public sealed record GetDistrictByIdResponse(
	Guid Id,
	Guid RegionId,
	IEnumerable<LanguageValue> Translates);
