using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.RoomTypes.Events;

public sealed record CreateOrUpdateRoomTypeDomainEvent(
	Guid RoomTypeId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
