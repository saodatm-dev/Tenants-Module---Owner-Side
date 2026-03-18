using Core.Domain.Events;
using Core.Domain.Languages;

namespace Building.Domain.Categories.Events;

public sealed record UpsertCategoryDomainEvent(
	Guid CategoryId,
	IEnumerable<LanguageValue> Translates) : IPrePublishDomainEvent;
