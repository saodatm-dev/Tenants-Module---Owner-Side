using Building.Domain.RealEstates;
using Building.Domain.Rooms;
using Building.Domain.Units;
using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Domain.RealEstates.Events;

public sealed record UpsertRealEstateDomainEvent(
	RealEstate RealEstate,
	IEnumerable<UnitRequest>? Units = null,
	IEnumerable<RoomValue>? Rooms = null,
	IEnumerable<LanguageValue>? Descriptions = null,
	IEnumerable<string>? Images = null,
	IEnumerable<Guid>? AmenityIds = null) : IPrePublishDomainEvent;
