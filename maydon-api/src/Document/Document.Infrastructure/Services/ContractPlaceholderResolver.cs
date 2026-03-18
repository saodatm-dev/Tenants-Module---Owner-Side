using Document.Contract.Gateways;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Services;

/// <summary>
/// Resolves all auto-resolvable placeholders for contract generation.
/// Aggregates data from Building (Lease, RealEstate) and Identity (Company, User, Bank) modules.
/// </summary>
public sealed class ContractPlaceholderResolver(
    IBuildingReadGateway buildingGateway,
    IIdentityDbContext identityDb) : IContractPlaceholderResolver
{
    public async Task<Dictionary<string, object?>> ResolveAllAsync(
        Guid leaseId,
        Guid tenantId,
        CancellationToken ct = default)
    {
        var values = new Dictionary<string, object?>();

        // ── 1. Lease data ──
        var lease = await buildingGateway.GetLeaseInfoAsync(leaseId, ct);
        if (lease is not null)
        {
            // Aggregate financials from all items
            var totalMonthlyRent = lease.Items.Sum(i => i.MonthlyRent.Amount);
            var totalDeposit = lease.Items.Sum(i => i.DepositAmount.Amount);

            values["lease_monthly_rent"] = totalMonthlyRent;
            values["lease_deposit_amount"] = totalDeposit;
            values["lease_payment_day"] = lease.PaymentDay;
            values["lease_start_date"] = lease.StartDate.ToString("dd.MM.yyyy");
            values["lease_end_date"] = lease.EndDate?.ToString("dd.MM.yyyy") ?? "";
            values["lease_contract_number"] = lease.ContractNumber ?? "";

            // Short-form aliases for template compatibility
            values["monthly_rent"] = totalMonthlyRent;
            values["deposit_amount"] = totalDeposit;
            values["payment_day"] = lease.PaymentDay;
            values["start_date"] = lease.StartDate.ToString("dd.MM.yyyy");
            values["end_date"] = lease.EndDate?.ToString("dd.MM.yyyy") ?? "";

            // ── 2. Real estate data (first item) ──
            var firstItem = lease.Items.FirstOrDefault();
            if (firstItem is not null)
            {
                var realEstate = await buildingGateway.GetRealEstateInfoAsync(firstItem.RealEstateId, ct);
                if (realEstate is not null)
                {
                    values["property_address"] = realEstate.Address ?? "";
                    values["property_cadastral_number"] = realEstate.CadastralNumber ?? "";
                    values["property_total_area"] = realEstate.TotalArea;
                }
            }

            // ── 3. Owner (landlord company) ──
            await ResolveCompanyAsync(tenantId, "owner", values, ct);

            // ── 4. Client (tenant company) ──
            await ResolveCompanyAsync(lease.ClientId, "client", values, ct);
        }

        // ── 5. Contract stubs ──
        values["contract_number"] = "б/н (черновик)";
        values["contract_date"] = DateTime.UtcNow.ToString("dd.MM.yyyy");

        return values;
    }

    private async Task ResolveCompanyAsync(
        Guid companyId,
        string prefix,
        Dictionary<string, object?> values,
        CancellationToken ct)
    {
        var company = await identityDb.Companies
            .AsNoTracking()
            .Where(c => c.Id == companyId)
            .Select(c => new
            {
                c.Name,
                c.Tin,
                c.Address,
                c.OwnerId
            })
            .FirstOrDefaultAsync(ct);

        if (company is null) return;

        values[$"{prefix}_company_name"] = company.Name;
        values[$"{prefix}_inn"] = company.Tin ?? "";
        values[$"{prefix}_address"] = company.Address ?? "";

        // Director (company owner)
        var director = await identityDb.Users
            .AsNoTracking()
            .Where(u => u.Id == company.OwnerId)
            .Select(u => new
            {
                FullName = (u.LastName ?? "") + " " + (u.FirstName ?? "") + " " + (u.MiddleName ?? ""),
                u.PhoneNumber,
                u.Pinfl
            })
            .FirstOrDefaultAsync(ct);

        if (director is not null)
        {
            values[$"{prefix}_director"] = director.FullName.Trim();
            values[$"{prefix}_phone"] = director.PhoneNumber ?? "";
            values[$"{prefix}_pinfl"] = director.Pinfl ?? "";
        }

        // Bank details (main account)
        var bank = await identityDb.BankProperties
            .AsNoTracking()
            .Where(b => b.TenantId == companyId && b.IsMain)
            .Select(b => new
            {
                b.BankName,
                b.BankMFO,
                b.AccountNumber
            })
            .FirstOrDefaultAsync(ct);

        if (bank is not null)
        {
            values[$"{prefix}_bank_name"] = bank.BankName;
            values[$"{prefix}_mfo"] = bank.BankMFO;
            values[$"{prefix}_bank_account"] = bank.AccountNumber;
        }
    }
}
