using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Complexes.Create;

public sealed record CreateComplexCommand(
	Guid RegionId,
	Guid DistrictId,
	string Name,
	IEnumerable<LanguageValue>? Descriptions,
	bool isCommercial,
	bool isLiving,
	double? Longitude = null,
	double? Latitude = null,
	string? Address = null,
	IEnumerable<string>? Images = null) : ICommand<Guid>;
