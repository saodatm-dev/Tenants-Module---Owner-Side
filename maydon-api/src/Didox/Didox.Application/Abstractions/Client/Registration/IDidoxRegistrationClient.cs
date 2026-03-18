using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using Didox.Application.Contracts.DidoxClient.Contracts.Registration;

namespace Didox.Application.Abstractions.Client.Registration;

public interface IDidoxRegistrationClient
{
    /// <summary>
    /// Registers a new user in the Didox system.
    /// </summary>
    /// <param name="request">
    /// User registration data, including email address, phone number, password, acceptance of terms, and digital signature.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the request execution.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with a user authorization token upon successful registration.
    /// </returns>
    Task<DidoxApiResponse<TokenResponse>> RegisterUserAsync(RegistrationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a digital signature with an attached time stamp token.
    /// </summary>
    /// <param name="request">
    /// Signature data including the PKCS#7 container and the signature hash in hexadecimal (HEX) format.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation.Used to terminate the request prematurely.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with the time stamp encoded in Base64 format and operation status information.
    /// </returns>
    Task<DidoxApiResponse<TimeStampTokenResponse>> CreateSignatureWithTimeStampAsync(Pkcs7SignatureRequest request, CancellationToken cancellationToken = default);
}
