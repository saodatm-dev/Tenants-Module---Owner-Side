using Core.Application.Abstractions.Notifications;
using Core.Application.Abstractions.Messaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Core.Infrastructure.Web;

/// <summary>
/// SignalR implementation of entity change notification handler.
/// Broadcasts entity changes to connected SignalR clients.
/// </summary>
public sealed class SignalREntityChangeNotificationHandler(
    IHubContext<VersioningHub> hubContext,
    ILogger<SignalREntityChangeNotificationHandler> logger) : IEntityChangeNotificationHandler
{
    private readonly IHubContext<VersioningHub> _hubContext = hubContext
                    ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly ILogger<SignalREntityChangeNotificationHandler> _logger = logger
                    ?? throw new ArgumentNullException(nameof(logger));

    public async Task HandleAsync(EntityVersionChangedEvent @event, CancellationToken cancellationToken)
    {
        try
        {
            await _hubContext.Clients
                .Group($"entity:{@event.EntityId}")
                .SendAsync("EntityChanged", @event, cancellationToken);

            _logger.LogDebug("Broadcasted version change for entity {EntityId} to SignalR clients", @event.EntityId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to broadcast version change for entity {EntityId}", @event.EntityId);
            throw;
        }
    }
}
