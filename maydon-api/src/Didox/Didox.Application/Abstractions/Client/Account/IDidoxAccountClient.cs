using Didox.Application.Contracts.DidoxClient.Contracts.Account;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

namespace Didox.Application.Abstractions.Client.Account;

public interface IDidoxAccountClient
{

    /// <summary>
    /// Retrieves account information for the currently authenticated user.
    /// </summary>
    /// <param name="userToken">
    /// The user token obtained after successful authentication. Used to identify the user within the system.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the request execution.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with the current user account information.
    /// </returns>
    Task<DidoxApiResponse<AccountResponse>> GetAccountInfoAsync(string userToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's account information.
    /// </summary>
    /// <param name="userToken">
    /// The user token confirming permission to modify account data.
    /// </param>
    /// <param name="request">
    /// Data used to update the user's account information (for example, contact details or notification settings).
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Used to terminate the request prematurely.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with the updated account information.
    /// </returns>
    Task<DidoxApiResponse<AccountResponse>> UpdateAccountInfoAsync(string userToken, ChangeAccountRequest request, CancellationToken cancellationToken = default);
    
}
