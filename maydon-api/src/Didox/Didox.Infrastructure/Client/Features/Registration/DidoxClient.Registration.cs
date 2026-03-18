using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;
using RestSharp;
// ReSharper disable CheckNamespace

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Didox Client - Registration Features
/// </summary>
public partial class DidoxClient
{
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<TokenResponse>> RegisterUserAsync(RegistrationRequest requestBody, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_registrUserUrl, Method.Post)
            .AddJsonBody(requestBody);

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<TokenResponse>(t.Result), cancellationToken);
    }

    
    /// <inheritdoc/>
    public async Task<DidoxApiResponse<TimeStampTokenResponse>> CreateSignatureWithTimeStampAsync(Pkcs7SignatureRequest requestBody, CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(_signatureUrl, Method.Post)
            .AddJsonBody(requestBody);

        return await RestClient.ExecuteAsync(request, cancellationToken)
            .ContinueWith(t => ProcessResponse<TimeStampTokenResponse>(t.Result), cancellationToken);
    }
}
