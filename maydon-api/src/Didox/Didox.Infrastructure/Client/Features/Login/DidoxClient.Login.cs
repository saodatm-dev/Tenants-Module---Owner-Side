using Didox.Application.Contracts.DidoxClient.Contracts.Auth;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using RestSharp;
// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Login related methods for DidoxClient
/// </summary>
public partial class DidoxClient
{
   
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<TokenResponse>> LoginWithPasswordAsync(string tinPini, string password, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_loginPasswordUrl.Replace("{tin}", tinPini), Method.Post)
            .AddJsonBody(new LoginRequest { Password = password });

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<TokenResponse>(t.Result), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<DidoxApiResponse<TokenResponse>> LoginWithSignatureAsync(string tinPini, string signature, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest( _loginSignatureUrl.Replace("{tin}", tinPini), Method.Post)
            .AddJsonBody(new LoginRequest { Signature = signature });

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<TokenResponse>(t.Result), cancellationToken);
    }
}
