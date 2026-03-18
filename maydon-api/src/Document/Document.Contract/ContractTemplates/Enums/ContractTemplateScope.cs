using System.Text.Json.Serialization;

namespace Document.Contract.ContractTemplates.Enums;

/// <summary>
/// Represents the scope of a contract template (Contract layer)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContractTemplateScope
{
    System = 0,
    Tenant = 1
}
