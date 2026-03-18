using Document.Contract.Enums;

namespace Document.Contract.SignalR;

/// <summary>
/// Service for sending real-time document status notifications via SignalR.
/// </summary>
public interface IDocumentStatusNotifier
{
    /// <summary>
    /// Notify about document status change.
    /// </summary>
    Task NotifyStatusChangeAsync(
        Guid documentId,
        DocumentStatus oldStatus,
        DocumentStatus newStatus,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Notify about export progress.
    /// </summary>
    Task NotifyExportProgressAsync(
        Guid documentId,
        string providerName,
        string status,
        int? progressPercentage = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify about export completion.
    /// </summary>
    Task NotifyExportCompletedAsync(
        Guid documentId,
        string providerName,
        bool success,
        string? externalId = null,
        string? errorMessage = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Notify about export failure.
    /// </summary>
    Task NotifyExportFailedAsync(
        Guid documentId,
        string providerName,
        string errorMessage,
        Guid? userId = null,
        CancellationToken cancellationToken = default);
}
