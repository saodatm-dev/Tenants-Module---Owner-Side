using System.Text.Json.Serialization;

namespace Didox.Application.Contracts.DidoxClient.Contracts.Documents.Factura.Response.Parties;

/// <summary>
/// Transaction party data (seller or buyer).
/// </summary>
public class FacturaParty
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("vatregcode")]
    public string? VatRegCode { get; set; }

    [JsonPropertyName("account")]
    public string? Account { get; set; }

    [JsonPropertyName("bankid")]
    public string? BankId { get; set; }

    [JsonPropertyName("address")]
    public string? Address { get; set; }

    [JsonPropertyName("vatregstatus")]
    public int VatRegStatus { get; set; }
}

