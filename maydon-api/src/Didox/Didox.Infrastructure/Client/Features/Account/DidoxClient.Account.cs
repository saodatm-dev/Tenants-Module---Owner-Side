using Didox.Application.Contracts.DidoxClient.Contracts.Account;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using RestSharp;
// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Didox Client - Account Features
/// </summary>
public partial class DidoxClient
{
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<AccountResponse>> GetAccountInfoAsync(string userToken, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_accountGetUrl)
            .AddHeader("user-key", userToken.Trim());

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<AccountResponse>(t.Result), cancellationToken);
    }
    
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<AccountResponse>> UpdateAccountInfoAsync(string userToken, ChangeAccountRequest requestBody, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_accountUpdateUrl, Method.Post)
            .AddHeader("user-key", userToken.Trim())
            .AddJsonBody(requestBody);

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<AccountResponse>(t.Result), cancellationToken);
    }
}
