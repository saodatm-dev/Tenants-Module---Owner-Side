namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Base record for integration events with built-in versioning support.
/// </summary>
public abstract record IntegrationEventBase : IIntegrationEvent
{
	public Guid EventId { get; init; } = Guid.NewGuid();
	public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
	public virtual int SchemaVersion => 1;
	public virtual string? EventTypeName => null;
}
