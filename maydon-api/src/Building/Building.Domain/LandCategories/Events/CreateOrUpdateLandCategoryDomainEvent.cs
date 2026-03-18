using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.LandCategories.Events;

public sealed record CreateOrUpdateLandCategoryDomainEvent(
	Guid LandCategoryId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
