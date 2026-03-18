using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Buildings.Update;

public sealed record UpdateBuildingCommand(
	Guid Id,
	string Number,
	IEnumerable<LanguageValue>? Descriptions,
	bool IsCommercial,
	bool IsLiving,
	Guid? RegionId,
	Guid? DistrictId,
	Guid? ComplexId = null,
	short? TotalSquare = null,
	short? FloorsCount = null,
	double? Latitude = null,
	double? Longitude = null,
	string? Address = null,
	IEnumerable<string>? Images = null) : ICommand<Guid>;
