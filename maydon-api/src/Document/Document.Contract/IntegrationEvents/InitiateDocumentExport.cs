using Core.Application.Abstractions.Messaging;

namespace Document.Contract.IntegrationEvents;

/// <summary>
/// Lightweight integration event to initiate document export process.
/// Published by command handler, consumed by preparation worker.
/// The preparation worker will then generate PDF, fetch data, and publish DocumentExportRequested.
/// V2: Added InitiatedBy to preserve user context in background processing
/// </summary>
public record InitiateDocumentExport(
    Guid DocumentId,
    string TargetProvider,
    Guid? InitiatedBy = null
) : IntegrationEventBase
{
    public override int SchemaVersion => 2;
}
