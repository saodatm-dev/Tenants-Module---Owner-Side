using Core.Domain.Entities;
using Document.Domain.ContractTemplates.Enums;

namespace Document.Domain.ContractTemplates;

public sealed class ContractTemplate : AggregateRoot<Guid>, ISoftDeleteEntity, IVersionedEntity
{
    public Guid? TenantId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public ContractTemplateScope Scope { get; set; }
    public ContractTemplateCategory Category { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Page { get; set; }
    public required string Theme { get; set; }
    public string? Header { get; set; }
    public string? Footer { get; set; }
    public required string Bodies { get; set; }
    public string? ManualFields { get; set; }

    public bool IsActive { get; set; } = true;
    public int CurrentVersion { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // ISoftDeleteEntity
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    private ContractTemplate() { }

    [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
    public ContractTemplate(
        Guid id,
        string code,
        string name,
        string page,
        string theme,
        string bodies)
    {
        Id = id;
        Code = code;
        Name = name;
        Page = page;
        Theme = theme;
        Bodies = bodies;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CurrentVersion = 1;
    }
}
