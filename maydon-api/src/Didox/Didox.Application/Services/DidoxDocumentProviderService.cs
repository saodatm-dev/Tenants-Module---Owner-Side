using System.Text.Json;
using Core.Application.Resources;
using Core.Domain.Results;
using Didox.Application.Abstractions.Client;
using Document.Contract.Gateways;
using Microsoft.Extensions.Logging;

namespace Didox.Application.Services;

/// <summary>
/// Didox implementation of document provider service.
/// Handles synchronous sign/reject operations with the Didox API.
/// Only responsible for API calls - document status updates handled by caller.
/// </summary>
public sealed class DidoxDocumentProviderService(
    IDidoxClient didoxClient,
    IDidoxAuthService authService,
    ISharedViewLocalizer localizer,
    ILogger<DidoxDocumentProviderService> logger)
    : IDocumentProviderService
{
    private readonly IDidoxClient _didoxClient = didoxClient ?? throw new ArgumentNullException(nameof(didoxClient));
    private readonly IDidoxAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));
    private readonly ISharedViewLocalizer _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    private readonly ILogger<DidoxDocumentProviderService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result> SignDocumentAsync(
        Guid documentId,
        string externalDocumentId,
        string providerName,
        string signatureData,
        string? signerTin,
        string? signerPinfl,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (providerName != "Didox")
        {
            return Result.Failure(_localizer.InvalidValue($"Provider '{providerName}' is not supported by this service"));
        }

        _logger.LogInformation("Calling Didox API to sign Document {DocumentId} - ExternalId: {ExternalId}", documentId, externalDocumentId);

        try
        {
            var tokenResult = await _authService.GetActiveTokenAsync(userId, cancellationToken);
            if (tokenResult.IsFailure)
            {
                _logger.LogError("Didox authentication failed for user {UserId}: {Error}", userId, tokenResult.Error.Description);
                return Result.Failure(_localizer.InvalidValue($"Authentication failed: {tokenResult.Error.Description}"));
            }

            var signResponse = await _didoxClient.SignDocumentAsync(
                signatureData,
                externalDocumentId,
                tokenResult.Value,
                cancellationToken);

            if (!signResponse.Success || !signResponse.Data)
            {
                var errorMsg = signResponse.ErrorMessage ?? "Unknown API error";
                _logger.LogError("Didox API failed to sign Document {DocumentId}: {Error}", documentId, errorMsg);
                return Result.Failure(_localizer.DidoxRequestFailed(errorMsg));
            }

            _logger.LogInformation("Didox API successfully signed Document {DocumentId}", documentId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception calling Didox API to sign Document {DocumentId}", documentId);
            return Result.Failure(_localizer.InternalServerError(ex.Message));
        }
    }

    public async Task<Result> RejectDocumentAsync(
        Guid documentId,
        string externalDocumentId,
        string providerName,
        string signatureData,
        string? rejectionReason,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        if (providerName != "Didox")
        {
            return Result.Failure(_localizer.InvalidValue($"Provider '{providerName}' is not supported by this service"));
        }

        _logger.LogInformation("Calling Didox API to reject Document {DocumentId} - ExternalId: {ExternalId}", documentId, externalDocumentId);

        try
        {
            var tokenResult = await _authService.GetActiveTokenAsync(userId, cancellationToken);
            if (tokenResult.IsFailure)
            {
                _logger.LogError("Didox authentication failed for user {UserId}: {Error}", userId, tokenResult.Error.Description);
                return Result.Failure(_localizer.InvalidValue($"Authentication failed: {tokenResult.Error.Description}"));
            }

            var rejectResponse = await _didoxClient.RejectDocumentAsync(
                signatureData,
                rejectionReason ?? "Rejected by user",
                externalDocumentId,
                tokenResult.Value,
                cancellationToken);

            if (!rejectResponse.Success || !rejectResponse.Data)
            {
                var errorMsg = rejectResponse.ErrorMessage ?? "Unknown API error";
                _logger.LogError("Didox API failed to reject Document {DocumentId}: {Error}", documentId, errorMsg);
                return Result.Failure(_localizer.DidoxRequestFailed(errorMsg));
            }

            _logger.LogInformation("Didox API successfully rejected Document {DocumentId}", documentId);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception calling Didox API to reject Document {DocumentId}", documentId);
            return Result.Failure(_localizer.InternalServerError(ex.Message));
        }
    }

    public async Task<Result<DocumentProviderStatus>> GetDocumentStatusAsync(
        string externalDocumentId,
        string providerName,
        Guid ownerUserId,
        CancellationToken cancellationToken = default)
    {
        if (providerName != "Didox")
        {
            return Result.Failure<DocumentProviderStatus>(
                _localizer.InvalidValue($"Provider '{providerName}' is not supported by this service"));
        }

        _logger.LogInformation("Fetching Didox document status for ExternalId: {ExternalId}", externalDocumentId);

        try
        {
            var tokenResult = await _authService.GetActiveTokenAsync(ownerUserId, cancellationToken);
            if (tokenResult.IsFailure)
            {
                _logger.LogError("Didox authentication failed for user {UserId}: {Error}", ownerUserId, tokenResult.Error.Description);
                return Result.Failure<DocumentProviderStatus>(
                    _localizer.InvalidValue($"Authentication failed: {tokenResult.Error.Description}"));
            }

            var docResponse = await _didoxClient.GetDidoxJsonDoc(
                externalDocumentId, tokenResult.Value, cancellationToken);

            if (!docResponse.Success || docResponse.Data is null)
            {
                var errorMsg = docResponse.ErrorMessage ?? "Unknown API error";
                _logger.LogError("Didox API failed to get document {ExternalId}: {Error}", externalDocumentId, errorMsg);
                return Result.Failure<DocumentProviderStatus>(_localizer.DidoxRequestFailed(errorMsg));
            }

            using var jsonDoc = docResponse.Data;
            var root = jsonDoc.RootElement;

            var rawStatus = root.TryGetProperty("status", out var statusProp)
                ? statusProp.GetString() ?? "unknown"
                : "unknown";

            var isSigned = rawStatus.Contains("signed", StringComparison.OrdinalIgnoreCase);
            var isRejected = rawStatus.Contains("rejected", StringComparison.OrdinalIgnoreCase) ||
                             rawStatus.Contains("cancelled", StringComparison.OrdinalIgnoreCase);

            string? rejectionReason = null;
            if (isRejected && root.TryGetProperty("rejectionReason", out var reasonProp))
                rejectionReason = reasonProp.GetString();

            DateTime? signedAt = null;
            if (isSigned && root.TryGetProperty("signedDate", out var signedDateProp) &&
                signedDateProp.TryGetDateTime(out var parsedDate))
                signedAt = parsedDate;

            var status = new DocumentProviderStatus(rawStatus, isSigned, isRejected, rejectionReason, signedAt);

            _logger.LogInformation("Didox document {ExternalId} status: {Status}", externalDocumentId, rawStatus);
            return Result.Success(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception fetching Didox document status for {ExternalId}", externalDocumentId);
            return Result.Failure<DocumentProviderStatus>(_localizer.InternalServerError(ex.Message));
        }
    }
}
