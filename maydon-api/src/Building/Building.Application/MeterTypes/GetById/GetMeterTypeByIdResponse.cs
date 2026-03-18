using Core.Domain.Languages;

namespace Building.Application.MeterTypes.GetById;

public sealed record GetMeterTypeByIdResponse(
	Guid Id,
	IEnumerable<LanguageValue> Names,
	IEnumerable<LanguageValue> Description,
	string? Icon);
