using Core.Application.Abstractions.Messaging;

namespace Core.Application.Abstractions.Notifications;

/// <summary>
/// Handler interface for processing entity change notifications.
/// Decouples Redis subscription from notification delivery mechanism.
/// </summary>
public interface IEntityChangeNotificationHandler
{
	Task HandleAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken);
}
