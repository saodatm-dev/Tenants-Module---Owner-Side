using Didox.Application.Abstractions.Client;
using Didox.Infrastructure.Client.Options;
using Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RestSharp;

namespace Didox.Infrastructure.Client.Features;

/// <summary>
/// Implementation of Didox HTTP client using RestSharp
/// Responsible ONLY for HTTP communication with Didox API
/// Split into partial classes by functionality for better organization
/// </summary>
public partial class DidoxClient : IDidoxClient
{
    protected readonly RestClient RestClient;
    protected readonly DidoxOptions Options;
    protected readonly ILogger<DidoxClient> Logger;
    
    private string _accountGetUrl = "/v1/account";
    private string _accountUpdateUrl = "/v1/profile/update";
    private string _loginPasswordUrl  = "/v1/auth/{tin}/password/ru";
    private string _loginSignatureUrl  = "/v1/auth/{tin}/token/ru";
    private string _registrUserUrl  = "/v1/auth/signup";
    private string _signatureUrl  = "/v1/dsvs/timestamp";
    private string _documentCreateUrl  = "/v1/documents/:docType/create";
    private string _documentSignUrl  = "/v1/documents/{documentId}/sign";
    private string _documentRejectUrl  = "/v1/documents/{documentId}/reject";
    private string _getHtmlDocumentUrl = "/v1/documents/{documentId}/html/ru";
    private string _getPdfDocumentUrl = "/v1/documents/{documentId}/pdf/ru";
    private string _getDocumentUrl = "/v1/documents/{documentId}?owner=1";

    public DidoxClient(HttpClient httpClient, 
        IOptions<DidoxOptions> options, 
        ILogger<DidoxClient> logger)
    {
        Logger = logger;
        Options = options.Value;
        RestClient = new RestClient(httpClient);
    }

    /// <summary>
    /// Helper method to process HTTP responses from Didox API
    /// </summary>
    protected DidoxApiResponse<T> ProcessResponse<T>(RestResponse response)
    {
        var result = new DidoxApiResponse<T>
        {
            StatusCode = response.StatusCode
        };
        
        if (string.IsNullOrWhiteSpace(response.Content))
        {
            result.Success = false;
            result.ErrorMessage = $"Empty response. HTTP {(int)response.StatusCode}";
            return result;
        }
        
        Logger.LogInformation("Received response. Content-Type={ContentType}", response.ContentType);

        var contentType = response.ContentType?.ToLowerInvariant() ?? string.Empty;
        
        if (!response.IsSuccessful)
        {
            result.Success = false;
            result.ErrorMessage = response.Content;
            return result;
        }
        
        // PDF response
        if (contentType.Contains("application/pdf"))
        {
            result.Success = true;
            result.Pdf = response.RawBytes;
            return result;
        }

        // HTML response
        if (contentType.Contains("text/html"))
        {
            result.Success = true;
            result.Html = response.Content;
            return result;
        }

        // JSON response
        if (contentType.Contains("application/json"))
        {
            try
            {
                if (typeof(T) == typeof(System.Text.Json.JsonDocument))
                {
                    var doc = System.Text.Json.JsonDocument.Parse(response.Content);
                    result.Data = (T)(object)doc;
                    result.Success = true;
                    return result;
                }
                
                result.Data = JsonSerializer.Deserialize<T>(response.Content)!;
                result.Success = true;
                return result;
            }
            catch (JsonException ex)
            {
                Logger.LogError(ex, "JSON deserialization failed");

                result.Success = false;
                result.ErrorMessage = "Failed to deserialize JSON response";
                return result;
            }
        }

        // Unknown content type
        result.Success = false;
        result.ErrorMessage = $"Unsupported content type: {response.ContentType}";
        return result;
    }
    
    /// <summary>
    /// Method to create a RestRequest with common headers
    /// </summary>
    private RestRequest CreateRequest(string url, Method method = Method.Get)
    {
        var request = new RestRequest(url, method)
            .AddHeader("Partner-Authorization", Options.PartnerToken.Trim())
            .AddHeader("Content-Type", "application/json");
        
        return request;
    }
}

