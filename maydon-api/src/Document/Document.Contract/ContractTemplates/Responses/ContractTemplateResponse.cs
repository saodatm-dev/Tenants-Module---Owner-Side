using System.Text.Json;
using Document.Contract.ContractTemplates.Enums;

namespace Document.Contract.ContractTemplates.Responses;

/// <summary>
/// Full contract template response with all jsonb fields.
/// </summary>
public sealed record ContractTemplateResponse
{
    public required Guid Id { get; init; }
    public Guid? TenantId { get; init; }
    public required Guid CreatedByUserId { get; init; }
    public required ContractTemplateScope Scope { get; init; }
    public required ContractTemplateCategory Category { get; init; }
    public required string Code { get; init; }
    public required JsonElement Name { get; init; }
    public JsonElement? Description { get; init; }
    public required JsonElement Page { get; init; }
    public required JsonElement Theme { get; init; }
    public JsonElement? Header { get; init; }
    public JsonElement? Footer { get; init; }
    public required JsonElement Bodies { get; init; }
    public JsonElement? ManualFields { get; init; }
    public required bool IsActive { get; init; }
    public required int CurrentVersion { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
