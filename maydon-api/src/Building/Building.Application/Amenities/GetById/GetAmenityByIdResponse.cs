using Core.Domain.Languages;

namespace Building.Application.Amenities.GetById;

public sealed record GetAmenityByIdResponse(
	Guid Id,
	string IconUrl,
	IEnumerable<LanguageValue> Translates);

