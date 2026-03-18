using Core.Domain.Events;

namespace Common.Domain.Currencies.Events;

public sealed record RemoveCurrencyDomainEvent(Guid Id) : IPrePublishDomainEvent;
