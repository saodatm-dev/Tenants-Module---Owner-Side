using Microsoft.AspNetCore.SignalR;

namespace Document.Infrastructure.SignalR;

/// <summary>
/// SignalR hub for real-time document status notifications.
/// Clients subscribe to document IDs and receive instant updates.
/// </summary>
public sealed class DocumentStatusHub : Hub
{
    public async Task SubscribeToDocument(Guid documentId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetDocumentGroup(documentId));
    }

    public async Task UnsubscribeFromDocument(Guid documentId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetDocumentGroup(documentId));
    }

    public async Task SubscribeToDocuments(Guid[] documentIds)
    {
        await Task.WhenAll(documentIds.Select(id => 
            Groups.AddToGroupAsync(Context.ConnectionId, GetDocumentGroup(id))));
    }

    public async Task SubscribeToUserDocuments(Guid userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetUserDocumentsGroup(userId));
    }

    private static string GetDocumentGroup(Guid documentId) => $"document:{documentId}";
    private static string GetUserDocumentsGroup(Guid userId) => $"user-documents:{userId}";
}
