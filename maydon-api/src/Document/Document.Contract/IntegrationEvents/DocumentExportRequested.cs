using Core.Application.Abstractions.Messaging;

namespace Document.Contract.IntegrationEvents;

/// <summary>
/// Integration event raised when a document export is requested to an external provider.
/// </summary>
public record DocumentExportRequested(
    Guid DocumentId,
    string TargetProvider,
    DocumentExportPayload Payload,
    Guid? InitiatedBy = null
) : IntegrationEventBase
{
    public override int SchemaVersion => 1;
}
