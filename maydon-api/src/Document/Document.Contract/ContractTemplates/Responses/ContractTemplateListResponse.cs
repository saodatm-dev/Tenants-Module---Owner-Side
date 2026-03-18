using Document.Contract.ContractTemplates.Enums;

namespace Document.Contract.ContractTemplates.Responses;

/// <summary>
/// Lightweight response for list endpoints (no jsonb body content).
/// </summary>
public sealed record ContractTemplateListResponse
{
    public required Guid Id { get; init; }
    public required string Code { get; init; }
    public required string Name { get; init; }
    public required ContractTemplateScope Scope { get; init; }
    public required ContractTemplateCategory Category { get; init; }
    public required bool IsActive { get; init; }
    public required int CurrentVersion { get; init; }
    public required DateTime CreatedAt { get; init; }
}
