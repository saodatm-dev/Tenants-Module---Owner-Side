using Core.Domain.Enums;

namespace Core.Application.Abstractions.Messaging;

/// <summary>
/// Integration event raised when a versioned entity is created or modified.
/// Published to Redis Streams for MongoDB persistence, hot storage, and SignalR notifications.
/// </summary>
public sealed record EntityVersionChangedEvent : IntegrationEventBase
{
	public required Guid EntityId { get; init; }
	public required string EntityType { get; init; }
	public required int VersionNumber { get; init; }
	public required string Data { get; init; }
	public Guid? ChangedBy { get; init; }
	public required DateTime Timestamp { get; init; }
	public required EntityChangeType ChangeType { get; init; }
	public string? ChangeDescription { get; init; }

	public override int SchemaVersion => 1;
}
