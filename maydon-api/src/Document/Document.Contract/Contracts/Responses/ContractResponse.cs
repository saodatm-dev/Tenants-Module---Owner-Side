using Core.Domain.ValueObjects;
using Document.Contract.Contracts.Enums;

namespace Document.Contract.Contracts.Responses;

/// <summary>
/// Response model for Contract entity.
/// </summary>
public sealed record ContractResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public string? ContractNumber { get; init; }
    public required Guid TemplateId { get; init; }
    public required string Language { get; init; }
    public required ContractStatusDto Status { get; init; }
    public required Guid LeaseId { get; init; }
    public required Guid RealEstateId { get; init; }
    public required Guid OwnerCompanyId { get; init; }
    public Guid? ClientCompanyId { get; init; }
    public string? OwnerInn { get; init; }
    public string? ClientInn { get; init; }
    public string? OwnerPinfl { get; init; }
    public string? ClientPinfl { get; init; }
    public required Money MonthlyAmount { get; init; }
    public required DateOnly LeaseStartDate { get; init; }
    public DateOnly? LeaseEndDate { get; init; }
    public required DateTime ContractDate { get; init; }
    public Guid? ParentId { get; init; }
    public required int CurrentVersion { get; init; }
    public string? RejectionReason { get; init; }
    public DateTime? SignatureDeadline { get; init; }
    public DateTime? ExportedAt { get; init; }
    public DateTime? OwnerSignedAt { get; init; }
    public DateTime? ClientSignedAt { get; init; }
    public required DateTime CreatedAt { get; init; }
    public IReadOnlyCollection<ContractFinancialItemResponse> FinancialItems { get; init; } = [];
}

public sealed record ContractFinancialItemResponse(
    Guid Id,
    string Type,
    string Name,
    Money Amount,
    string Frequency,
    int SortOrder);
