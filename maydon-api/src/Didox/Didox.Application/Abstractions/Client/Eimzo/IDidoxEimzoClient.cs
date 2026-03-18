using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

namespace Didox.Application.Abstractions.Client.Eimzo;

public interface IDidoxEimzoClient
{
    /// <summary>
    /// Signs an electronic document using an E-IMZO digital signature.
    /// </summary>
    /// <param name="signature">
    /// The E-IMZO digital signature generated on the client side. Used to confirm the authenticity and integrity of the document.
    /// </param>
    /// <param name="documentId">
    /// The unique identifier of the document in the Didox system that needs to be signed.
    /// </param>
    /// <param name="userToken">User's token on Didox system</param>
    /// <param name="cancellationToken">
    /// A token to cancel the asynchronous operation. Allows aborting the request if necessary.
    /// </param>
    /// <returns>
    /// The task result contains the Didox API response with a boolean value:<c>true</c> � the document was successfully signed; <c>false</c> � the operation failed.
    /// </returns>
    Task<DidoxApiResponse<bool>> SignDocumentAsync(string signature, string documentId, string userToken, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Reject an electronic document using an E-IMZO digital signature.
    /// </summary>
    /// <param name="signature">The E-IMZO digital signature generated on the client side. Used to confirm the authenticity and integrity of the document.</param>
    /// <param name="comment"></param>
    /// <param name="documentId">The unique identifier of the document in the Didox system that needs to be signed.</param>
    /// <param name="tokenValue">User's token on Didox system</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation. Allows aborting the request if necessary.</param>
    /// <returns></returns>
    Task<DidoxApiResponse<bool>> RejectDocumentAsync(string signature, string comment, string documentId, string tokenValue, CancellationToken cancellationToken);
}
