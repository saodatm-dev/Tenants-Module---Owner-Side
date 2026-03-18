using Document.Contract.Contracts.Enums;
using Document.Contract.Contracts.Responses;
using Document.Domain.Contracts;
using Document.Domain.Contracts.Enums;

namespace Document.Application.Features.Contracts.Mappings;

/// <summary>
/// Mapping extensions for Contract domain entities to response DTOs.
/// </summary>
public static class ContractMappings
{
    public static ContractResponse ToResponse(this Domain.Contracts.Contract contract) => new()
    {
        Id = contract.Id,
        TenantId = contract.TenantId,
        ContractNumber = contract.ContractNumber,
        TemplateId = contract.TemplateId,
        Language = contract.Language,
        Status = contract.Status.ToDto(),
        LeaseId = contract.LeaseId,
        RealEstateId = contract.RealEstateId,
        OwnerCompanyId = contract.OwnerCompanyId,
        ClientCompanyId = contract.ClientCompanyId,
        OwnerInn = contract.OwnerInn,
        ClientInn = contract.ClientInn,
        OwnerPinfl = contract.OwnerPinfl,
        ClientPinfl = contract.ClientPinfl,
        MonthlyAmount = contract.MonthlyAmount,
        LeaseStartDate = contract.LeaseStartDate,
        LeaseEndDate = contract.LeaseEndDate,
        ContractDate = contract.ContractDate,
        ParentId = contract.ParentId,
        CurrentVersion = contract.CurrentVersion,
        RejectionReason = contract.RejectionReason,
        SignatureDeadline = contract.SignatureDeadline,
        ExportedAt = contract.ExportedAt,
        OwnerSignedAt = contract.OwnerSignedAt,
        ClientSignedAt = contract.ClientSignedAt,
        CreatedAt = contract.ContractDate,
        FinancialItems = contract.FinancialItems
            .OrderBy(fi => fi.SortOrder)
            .Select(fi => new ContractFinancialItemResponse(
                fi.Id,
                fi.Type.ToString(),
                fi.Name,
                fi.Amount,
                fi.Frequency.ToString(),
                fi.SortOrder))
            .ToList()
    };

    public static ContractStatusDto ToDto(this ContractStatus status) =>
        (ContractStatusDto)(int)status;

    public static ContractStatus ToDomain(this ContractStatusDto status) =>
        (ContractStatus)(int)status;
}
