using Core.Domain.Events;

namespace Common.Domain.Banks.Events;

public sealed record RemoveBankDomainEvent(Guid Id) : IPrePublishDomainEvent;
