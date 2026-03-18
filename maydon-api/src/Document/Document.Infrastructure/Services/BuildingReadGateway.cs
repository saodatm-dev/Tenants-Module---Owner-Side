using Building.Application.Core.Abstractions.Data;
using Document.Contract.Gateways;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Services;

/// <summary>
/// Reads Lease and RealEstate data from the Building module's DbContext.
/// Follows the same pattern as <see cref="UserLookupService"/> which reads from Identity.
/// </summary>
public sealed class BuildingReadGateway(IBuildingDbContext buildingDbContext) : IBuildingReadGateway
{
    public async Task<LeaseInfo?> GetLeaseInfoAsync(Guid leaseId, CancellationToken ct = default)
    {
        return await buildingDbContext.Leases
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(l => l.Id == leaseId)
            .Include(l => l.Items)
            .Select(l => new LeaseInfo(
                l.Id,
                l.OwnerId,
                l.ClientId,
                l.PaymentDay,
                l.StartDate,
                l.EndDate,
                l.ContractNumber,
                l.Items.Select(item => new LeaseItemInfo(
                    item.RealEstateId,
                    item.RealEstateUnitId,
                    item.MonthlyRent,
                    item.DepositAmount)).ToList()))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<RealEstateInfo?> GetRealEstateInfoAsync(Guid realEstateId, CancellationToken ct = default)
    {
        return await buildingDbContext.RealEstates
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(r => r.Id == realEstateId)
            .Select(r => new RealEstateInfo(
                r.Id,
                r.OwnerId,
                r.Address,
                r.CadastralNumber,
                r.TotalArea,
                r.RealEstateTypeId))
            .FirstOrDefaultAsync(ct);
    }
}
