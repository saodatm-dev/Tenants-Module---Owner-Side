namespace Document.Contract.IntegrationEvents;

/// <summary>
/// Lightweight DTO carrying only the fields needed for external export.
/// </summary>
public sealed record ExportDocumentDto(
    Guid Id,
    string? DocumentNumber,
    DateTimeOffset ContractDate,
    string? SignerTin,
    string? SignerPinfl);

public record DocumentExportPayload(
    ExportDocumentDto Document,
    OwnerDto Owner,
    OwnerDto Signer,
    string TargetProvider,
    string? PdfBase64 = null);

public record OwnerDto(Guid Id, string Name, string Tin, string Pinfl, string? Address = null);

