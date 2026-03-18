using Core.Domain.Results;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;

namespace Didox.Application.Services;

/// <summary>
/// Abstraction for retrieving active Didox tokens for a specific owner.
/// Designed to be used from background jobs where there is no current HTTP user.
/// </summary>
public interface IDidoxAuthService
{
    /// <summary>
    /// Returns an active Didox token for the given owner (user) id,
    /// refreshing it from Didox if necessary.
    /// </summary>
    Task<Result<string>> GetActiveTokenAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Returns an active Didox token for the given owner (user) id,
    /// refreshing it from Didox if necessary.
    /// </summary>
    /// <param name="ownerId">User's unique identifier</param>
    /// <param name="cancellationToken">Token to cancel operation.</param>
    /// <returns></returns>
    Task<Result<string>> GetActiveTokenAsync(Guid ownerId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Will perform login to a Didox system for the specified owner
    /// </summary>
    public Task<Result<DidoxAccountResponse>> RegisterAsync(RegistrationRequest request, CancellationToken cancellationToken = default);
}


