using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

namespace Didox.Application.Abstractions.Client.Login;

public interface IDidoxLoginClient
{
    /// <summary>
    /// Authenticates a user in the Didox system using a password.
    /// </summary>
    /// <param name="tinPini">
    /// The user identifier (TIN or PINFL), used to uniquely identify the account.
    /// </param>
    /// <param name="password">
    /// The user's password used for authentication.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the HTTP request execution.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with an access token upon successful authentication.
    /// </returns>
    Task<DidoxApiResponse<TokenResponse>> LoginWithPasswordAsync(string tinPini, string password, CancellationToken cancellationToken = default);
        
    /// <summary>
    /// Authenticates a user in the Didox system using a digital signature (E-IMZO).
    /// </summary>
    /// <param name="tinPini">
    /// The user identifier (TIN or PINFL) for which authentication is performed.
    /// </param>
    /// <param name="signature">
    /// The digital signature (E-IMZO) generated on the client side and used to verify the user's identity.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Used to terminate the request prematurely.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with an access token upon successful signature-based authentication.
    /// </returns>
    Task<DidoxApiResponse<TokenResponse>> LoginWithSignatureAsync(string tinPini, string signature, CancellationToken cancellationToken = default);
}
