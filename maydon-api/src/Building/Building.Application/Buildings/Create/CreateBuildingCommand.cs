using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;

namespace Building.Application.Buildings.Create;

public sealed record CreateBuildingCommand(
	bool IsCommercial,
	bool IsLiving,
	string Number,
	Guid? RegionId,
	Guid? DistrictId,
	Guid? ComplexId = null,
	short? TotalArea = null,
	short? FloorsCount = null,
	double? Latitude = null,
	double? Longitude = null,
	string? Address = null,
	IEnumerable<LanguageValue>? Descriptions = null,
	IEnumerable<string>? Images = null) : ICommand<Guid>;
