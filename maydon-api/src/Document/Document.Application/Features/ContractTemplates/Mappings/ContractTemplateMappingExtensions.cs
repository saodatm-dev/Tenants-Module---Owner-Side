using System.Text.Json;
using Document.Contract.ContractTemplates.Responses;
using Document.Domain.ContractTemplates;
using ContractScope = Document.Contract.ContractTemplates.Enums.ContractTemplateScope;
using ContractCategory = Document.Contract.ContractTemplates.Enums.ContractTemplateCategory;
using DomainScope = Document.Domain.ContractTemplates.Enums.ContractTemplateScope;
using DomainCategory = Document.Domain.ContractTemplates.Enums.ContractTemplateCategory;

namespace Document.Application.Features.ContractTemplates.Mappings;

public static class ContractTemplateMappingExtensions
{
    /// <summary>
    /// Maps the domain entity to a full response DTO (for GetById).
    /// </summary>
    public static ContractTemplateResponse ToResponse(this ContractTemplate entity)
    {
        return new ContractTemplateResponse
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            CreatedByUserId = entity.CreatedByUserId,
            Scope = entity.Scope.ToContract(),
            Category = entity.Category.ToContract(),
            Code = entity.Code,
            Name = ParseJsonElement(entity.Name),
            Description = entity.Description is not null ? ParseJsonElement(entity.Description) : null,
            Page = ParseJsonElement(entity.Page),
            Theme = ParseJsonElement(entity.Theme),
            Header = entity.Header is not null ? ParseJsonElement(entity.Header) : null,
            Footer = entity.Footer is not null ? ParseJsonElement(entity.Footer) : null,
            Bodies = ParseJsonElement(entity.Bodies),
            ManualFields = entity.ManualFields is not null ? ParseJsonElement(entity.ManualFields) : null,
            IsActive = entity.IsActive,
            CurrentVersion = entity.CurrentVersion,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
    }

    /// <summary>
    /// Maps the domain entity to a lightweight list response DTO.
    /// Resolves multlingual Name JSON to the requested language.
    /// </summary>
    public static ContractTemplateListResponse ToListResponse(
        this ContractTemplate entity,
        string languageCode)
    {
        return new ContractTemplateListResponse
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = ResolveLocalizedName(entity.Name, languageCode),
            Scope = entity.Scope.ToContract(),
            Category = entity.Category.ToContract(),
            IsActive = entity.IsActive,
            CurrentVersion = entity.CurrentVersion,
            CreatedAt = entity.CreatedAt,
        };
    }

    // ── Enum mapping ───────────────────────────────────

    public static DomainScope ToDomain(this ContractScope scope) =>
        scope switch
        {
            ContractScope.System => DomainScope.System,
            ContractScope.Tenant => DomainScope.Tenant,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };

    public static ContractScope ToContract(this DomainScope scope) =>
        scope switch
        {
            DomainScope.System => ContractScope.System,
            DomainScope.Tenant => ContractScope.Tenant,
            _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
        };

    public static DomainCategory ToDomain(this ContractCategory category) =>
        category switch
        {
            ContractCategory.LeaseAgreement => DomainCategory.LeaseAgreement,
            ContractCategory.Sublease => DomainCategory.Sublease,
            ContractCategory.Commercial => DomainCategory.Commercial,
            ContractCategory.Service => DomainCategory.Service,
            ContractCategory.Custom => DomainCategory.Custom,
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };

    public static ContractCategory ToContract(this DomainCategory category) =>
        category switch
        {
            DomainCategory.LeaseAgreement => ContractCategory.LeaseAgreement,
            DomainCategory.Sublease => ContractCategory.Sublease,
            DomainCategory.Commercial => ContractCategory.Commercial,
            DomainCategory.Service => ContractCategory.Service,
            DomainCategory.Custom => ContractCategory.Custom,
            _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
        };

    // ── Private helpers ─────────────────────────────────

    private static JsonElement ParseJsonElement(string json)
    {
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.Clone();
    }

    private static string ResolveLocalizedName(string nameJson, string languageCode)
    {
        try
        {
            using var doc = JsonDocument.Parse(nameJson);
            var root = doc.RootElement;

            // Try the requested language first, then fall back to "ru", then first available
            if (root.TryGetProperty(languageCode, out var langProp) && langProp.ValueKind == JsonValueKind.String)
                return langProp.GetString() ?? "";

            if (root.TryGetProperty("ru", out var ruProp) && ruProp.ValueKind == JsonValueKind.String)
                return ruProp.GetString() ?? "";

            // Return first available string value
            foreach (var prop in root.EnumerateObject())
            {
                if (prop.Value.ValueKind == JsonValueKind.String)
                    return prop.Value.GetString() ?? "";
            }

            return "";
        }
        catch
        {
            // If the Name is just a plain string (not JSON), return it as-is
            return nameJson;
        }
    }
}
