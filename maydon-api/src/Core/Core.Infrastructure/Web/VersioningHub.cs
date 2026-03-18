using Microsoft.AspNetCore.SignalR;

namespace Core.Infrastructure.Web;

/// <summary>
/// SignalR hub for real-time entity version change notifications.
/// Clients subscribe to specific entities and receive updates when they change.
/// </summary>
public sealed class VersioningHub : Hub
{
    /// <summary>
    /// Subscribe to receive notifications for a specific entity.
    /// </summary>
    public async Task SubscribeToEntity(Guid entityId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(entityId));
    }

    /// <summary>
    /// Unsubscribe from entity notifications.
    /// </summary>
    public async Task UnsubscribeFromEntity(Guid entityId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(entityId));
    }

    /// <summary>
    /// Subscribe to multiple entities at once.
    /// </summary>
    public async Task SubscribeToEntities(Guid[] entityIds)
    {
        await Task.WhenAll(entityIds.Select(id => 
            Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(id))));
    }

    /// <summary>
    /// Unsubscribe from multiple entities at once.
    /// </summary>
    public async Task UnsubscribeFromEntities(Guid[] entityIds)
    {
        await Task.WhenAll(entityIds.Select(id => 
            Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(id))));
    }

    private static string GetGroupName(Guid entityId) => $"entity:{entityId}";
}
