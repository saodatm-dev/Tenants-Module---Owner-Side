using System.Text.Json.Serialization;

namespace Document.Contract.Enums;

/// <summary>
/// Represents Didox document types (Contract copy for external services)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocType
{
    ArbitraryDocument = 000,
    InvoiceWithOutAct = 002,
    ActCompleteWork = 005,
    InvoiceFarm = 008,
    Contract = 007
}

