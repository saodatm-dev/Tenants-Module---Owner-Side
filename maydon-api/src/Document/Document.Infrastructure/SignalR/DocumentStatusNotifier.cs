using Document.Contract.Enums;
using Document.Contract.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Document.Infrastructure.SignalR;

/// <summary>
/// SignalR-based implementation for real-time document status notifications.
/// Supports both document-specific and user-wide broadcasts.
/// </summary>
public sealed class DocumentStatusNotifier(
    IHubContext<DocumentStatusHub> hubContext,
    ILogger<DocumentStatusNotifier> logger)
    : IDocumentStatusNotifier
{
    private readonly IHubContext<DocumentStatusHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    private readonly ILogger<DocumentStatusNotifier> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task NotifyStatusChangeAsync(
        Guid documentId,
        DocumentStatus oldStatus,
        DocumentStatus newStatus,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                DocumentId = documentId,
                OldStatus = oldStatus.ToString(),
                NewStatus = newStatus.ToString(),
                Timestamp = DateTime.UtcNow,
                Type = "StatusChanged"
            };

            var tasks = new List<Task>
            {
                _hubContext.Clients
                    .Group($"document:{documentId}")
                    .SendAsync("DocumentStatusChanged", notification, cancellationToken)
            };

            if (userId.HasValue)
            {
                tasks.Add(_hubContext.Clients
                    .Group($"user-documents:{userId.Value}")
                    .SendAsync("DocumentStatusChanged", notification, cancellationToken));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation("Notified status change for Document {DocumentId}: {OldStatus} -> {NewStatus}", documentId, oldStatus, newStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send status change notification for Document {DocumentId}", documentId);
        }
    }

    public async Task NotifyExportProgressAsync(
        Guid documentId,
        string providerName,
        string status,
        int? progressPercentage = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                DocumentId = documentId,
                ProviderName = providerName,
                Status = status,
                Progress = progressPercentage,
                Timestamp = DateTime.UtcNow,
                Type = "ExportProgress"
            };

            var tasks = new List<Task>
            {
                _hubContext.Clients
                    .Group($"document:{documentId}")
                    .SendAsync("DocumentExportProgress", notification, cancellationToken)
            };

            if (userId.HasValue)
            {
                tasks.Add(_hubContext.Clients
                    .Group($"user-documents:{userId.Value}")
                    .SendAsync("DocumentExportProgress", notification, cancellationToken));
            }

            await Task.WhenAll(tasks);

            _logger.LogDebug(
                "Notified export progress for Document {DocumentId} to {Provider}: {Status} ({Progress}%)",
                documentId, providerName, status, progressPercentage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send export progress notification for Document {DocumentId}", documentId);
        }
    }

    public async Task NotifyExportCompletedAsync(
        Guid documentId,
        string providerName,
        bool success,
        string? externalId = null,
        string? errorMessage = null,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = new
            {
                DocumentId = documentId,
                ProviderName = providerName,
                Success = success,
                ExternalId = externalId,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow,
                Type = "ExportCompleted"
            };

            var tasks = new List<Task>
            {
                _hubContext.Clients
                    .Group($"document:{documentId}")
                    .SendAsync("DocumentExportCompleted", notification, cancellationToken)
            };

            if (userId.HasValue)
            {
                tasks.Add(_hubContext.Clients
                    .Group($"user-documents:{userId.Value}")
                    .SendAsync("DocumentExportCompleted", notification, cancellationToken));
            }

            await Task.WhenAll(tasks);

            _logger.LogInformation(
                "Notified export completion for Document {DocumentId} to {Provider}: Success={Success}, ExternalId={ExternalId}",
                documentId, providerName, success, externalId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to send export completion notification for Document {DocumentId}",
                documentId);
        }
    }

    public async Task NotifyExportFailedAsync(
        Guid documentId,
        string providerName,
        string errorMessage,
        Guid? userId = null,
        CancellationToken cancellationToken = default)
    {
        await NotifyExportCompletedAsync(
            documentId,
            providerName,
            success: false,
            externalId: null,
            errorMessage: errorMessage,
            userId: userId,
            cancellationToken: cancellationToken);
    }
}
