using Core.Domain.Results;

namespace Document.Contract.Gateways;

/// <summary>
/// Service for interacting with external document providers (Didox, ESign, etc.)
/// for signing, rejection, and status polling operations.
/// </summary>
public interface IDocumentProviderService
{
    /// <summary>
    /// Signs a document in the external provider system.
    /// </summary>
    Task<Result> SignDocumentAsync(
        Guid documentId,
        string externalDocumentId,
        string providerName,
        string signatureData,
        string? signerTin,
        string? signerPinfl,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects a document in the external provider system.
    /// </summary>
    Task<Result> RejectDocumentAsync(
        Guid documentId,
        string externalDocumentId,
        string providerName,
        string signatureData,
        string? rejectionReason,
        Guid userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current status of a document from the external provider.
    /// </summary>
    /// <param name="externalDocumentId">External provider document ID</param>
    /// <param name="providerName">Name of the provider (Didox, ESign, etc.)</param>
    /// <param name="ownerUserId">Owner user ID (for authentication)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current document status from the provider</returns>
    Task<Result<DocumentProviderStatus>> GetDocumentStatusAsync(
        string externalDocumentId,
        string providerName,
        Guid ownerUserId,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Status information returned from an external document provider.
/// </summary>
public sealed record DocumentProviderStatus(
    string RawStatus,
    bool IsSigned,
    bool IsRejected,
    string? RejectionReason,
    DateTime? SignedAt);
