using Didox.Application.Abstractions.Client;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services;
using Didox.Application.Services;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CommonModels;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.Root;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Request.MetaData;
using Didox.Application.Contracts.DidoxClient.Contracts.Documents.CustomDocument.Common;
using Document.Contract.Enums;
using Document.Contract.IntegrationEvents;
using Document.Contract.SignalR;
using Microsoft.Extensions.Logging;
// ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract

namespace Didox.Infrastructure.Workers;

/// <summary>
/// Handler for exporting documents to Didox provider. Uses idempotent base class to prevent duplicate exports.
/// </summary>
public sealed class DidoxExportWorker(
    IDidoxClient didoxClient,
    IDidoxAuthService authService,
    IIntegrationEventPublisher publisher,
    IDocumentStatusNotifier statusNotifier,
    IBackgroundUserContext backgroundUserContext,
    IIdempotencyStore idempotencyStore,
    ILogger<DidoxExportWorker> logger)
    : IdempotentIntegrationEventHandlerBase<DocumentExportRequested>(idempotencyStore, logger)
{
    private readonly IDidoxClient _didoxClient = didoxClient ?? throw new ArgumentNullException(nameof(didoxClient));
    private readonly IDidoxAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    private readonly IIntegrationEventPublisher _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    private readonly IDocumentStatusNotifier _statusNotifier = statusNotifier ?? throw new ArgumentNullException(nameof(statusNotifier));
    private readonly IBackgroundUserContext _backgroundUserContext = backgroundUserContext ?? throw new ArgumentNullException(nameof(backgroundUserContext));

    protected override async Task HandleIdempotentAsync(DocumentExportRequested @event, CancellationToken ct)
    {
        _backgroundUserContext.SetUser(@event.InitiatedBy);
        
        if (@event.TargetProvider != "Didox")
        {
            Logger.LogDebug("Skipping non-Didox export for Document {DocumentId}", @event.DocumentId);
            _backgroundUserContext.Clear();
            return;
        }

        Logger.LogInformation("Processing Didox export for Document {DocumentId} - Owner: {OwnerId}, InitiatedBy: {InitiatedBy}",
            @event.DocumentId, @event.Payload.Owner.Id, @event.InitiatedBy);

        try
        {
            await _statusNotifier.NotifyExportProgressAsync(@event.DocumentId, "Didox", "Authenticating with Didox", 10, @event.Payload.Owner.Id, ct);
            
            var tokenResult = await _authService.GetActiveTokenAsync(@event.Payload.Owner.Id, ct);
            if (tokenResult.IsFailure)
            {
                var errorMsg = $"Authentication failed: {tokenResult.Error.Description}";
                Logger.LogError(errorMsg);
                await PublishStatusAsync(@event, ExternalSyncStatus.Failed, errorMsg, ct);
               
                await _statusNotifier.NotifyExportFailedAsync(@event.DocumentId, "Didox", errorMsg, @event.Payload.Owner.Id, ct);
                return;
            }
            
            await _statusNotifier.NotifyExportProgressAsync(@event.DocumentId, "Didox", "Authenticated successfully", 30, @event.Payload.Owner.Id, ct);
            
            var payload = @event.Payload;
            if (string.IsNullOrEmpty(payload.PdfBase64))
            {
                Logger.LogError("PDF content missing for Document {DocumentId}", @event.DocumentId);
                await PublishStatusAsync(@event, ExternalSyncStatus.Failed, "PDF content is required", ct);
                
                await _statusNotifier.NotifyExportFailedAsync(@event.DocumentId, "Didox", "PDF content is required", @event.Payload.Owner.Id, ct);
                return;
            }

            // 3. Build Didox request
            var request = new DocumentUploadRequest
            {
                Document = $"data:application/pdf;base64,{payload.PdfBase64}",
                Data = new DocumentData
                {
                    Subtype = 3,
                    SellerTin = payload.Owner.Tin ?? payload.Owner.Pinfl ?? string.Empty,
                    BuyerTin = payload.Document.SignerTin ?? payload.Document.SignerPinfl ?? string.Empty,
                    Document = new DocumentInfo
                    {
                        DocumentNo = payload.Document.DocumentNumber ?? string.Empty,
                        DocumentName = "Contract",
                        DocumentDate = payload.Document.ContractDate.ToString("yyyy-MM-dd")
                    },
                    Buyer = new SimpleParty { Name = payload.Signer.Name },
                    Seller = new SimpleParty { Name = payload.Owner.Name },
                    ContractDoc = new ContractDoc
                    {
                        ContractDate = payload.Document.ContractDate.ToString("yyyy-MM-dd"),
                        ContractNo = payload.Document.DocumentNumber ?? string.Empty
                    }
                }
            };
            
            await _statusNotifier.NotifyExportProgressAsync(@event.DocumentId, "Didox", "Uploading document to Didox", 60, @event.Payload.Owner.Id, ct);
            
            var response = await _didoxClient.CreateCustomDocumentAsync(request, tokenResult.Value, ct);
            
            if (response is { Success: true, Data: not null })
            {
                Logger.LogInformation("Successfully exported Document {DocumentId} to Didox - ExternalId: {ExternalId}", @event.DocumentId, response.Data.Id);
                await PublishStatusAsync(@event, ExternalSyncStatus.Sent, null, ct, response.Data.Id);
                
                await _statusNotifier.NotifyExportCompletedAsync(
                    @event.DocumentId, 
                    "Didox", 
                    success: true, 
                    externalId: response.Data.Id, 
                    errorMessage: null,
                    userId: @event.Payload.Owner.Id, 
                    ct);
            }
            else
            {
                var errorMsg = response.ErrorMessage ?? "Unknown API error";
                Logger.LogError("Failed to export Document {DocumentId} to Didox: {Error}", @event.DocumentId, errorMsg);
                
                await PublishStatusAsync(@event, ExternalSyncStatus.Failed, errorMsg, ct);
                await _statusNotifier.NotifyExportFailedAsync(@event.DocumentId, "Didox", errorMsg, @event.Payload.Owner.Id, ct);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unhandled exception during Didox export for Document {DocumentId}", @event.DocumentId);
            await PublishStatusAsync(@event, ExternalSyncStatus.Failed, ex.Message, ct);
            
            await _statusNotifier.NotifyExportFailedAsync(@event.DocumentId, "Didox", $"Export error: {ex.Message}", @event.Payload.Owner.Id, ct);
            throw;
        }
        finally
        {
            _backgroundUserContext.Clear();
        }
    }

    private async Task PublishStatusAsync(
        DocumentExportRequested request, 
        ExternalSyncStatus status, 
        string? error, 
        CancellationToken ct, 
        string? externalId = null)
    {
        var statusEvent = new ProviderStatusChanged(
            DocumentId: request.DocumentId,
            ProviderName: "Didox",
            ExternalId: externalId,
            Status: status,
            ErrorMessage: error,
            InitiatedBy: request.InitiatedBy
        );
        
        await _publisher.PublishAsync(statusEvent, ct);
    }
}
