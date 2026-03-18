using System.Net;

namespace Didox.Application.Contracts.DidoxClient.Contracts.CommonModules;

/// <summary>
/// Generic response wrapper from Didox API
/// </summary>
public record DidoxApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Html { get; set; }
    public byte[]? Pdf { get; set; } 
    public string? ErrorMessage { get; set; }
    public HttpStatusCode StatusCode { get; set; }
}
