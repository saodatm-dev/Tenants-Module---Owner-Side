using Core.Domain.ValueObjects;

namespace Document.Contract.Gateways;

/// <summary>
/// Read-only gateway to access Lease and RealEstate data from the Building module.
/// Avoids direct project references from Document.Application to Building.Domain.
/// </summary>
public interface IBuildingReadGateway
{
    /// <summary>
    /// Gets lease information with related real estate data for contract generation.
    /// </summary>
    Task<LeaseInfo?> GetLeaseInfoAsync(Guid leaseId, CancellationToken ct = default);

    /// <summary>
    /// Gets real estate information by ID.
    /// </summary>
    Task<RealEstateInfo?> GetRealEstateInfoAsync(Guid realEstateId, CancellationToken ct = default);
}

/// <summary>
/// Snapshot of lease data needed for contract generation.
/// </summary>
public sealed record LeaseInfo(
    Guid LeaseId,
    Guid OwnerId,
    Guid ClientId,
    short PaymentDay,
    DateOnly StartDate,
    DateOnly? EndDate,
    string? ContractNumber,
    IReadOnlyList<LeaseItemInfo> Items);

/// <summary>
/// Snapshot of a single lease item (property) for contract generation.
/// </summary>
public sealed record LeaseItemInfo(
    Guid RealEstateId,
    Guid? RealEstateUnitId,
    Money MonthlyRent,
    Money DepositAmount);

/// <summary>
/// Snapshot of real estate data needed for contract generation.
/// </summary>
public sealed record RealEstateInfo(
    Guid RealEstateId,
    Guid OwnerId,
    string? Address,
    string? CadastralNumber,
    float? TotalArea,
    Guid RealEstateTypeId);
