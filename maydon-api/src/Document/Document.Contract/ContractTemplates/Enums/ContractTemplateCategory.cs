using System.Text.Json.Serialization;

namespace Document.Contract.ContractTemplates.Enums;

/// <summary>
/// Represents the category of a contract template (Contract layer)
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ContractTemplateCategory
{
    LeaseAgreement = 0,
    Sublease = 1,
    Commercial = 2,
    Service = 3,
    Custom = 99
}
