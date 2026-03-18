using Document.Contract.Gateways;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Services;

/// <summary>
/// Resolves owner/company/bank placeholders from the Identity module.
/// Queries Company, BankProperty (main), and User (company owner) tables.
/// </summary>
public sealed class OwnerPlaceholderResolver(IIdentityDbContext identityDb) : IOwnerPlaceholderResolver
{
    public async Task<Dictionary<string, object?>> ResolveOwnerValuesAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var values = new Dictionary<string, object?>();

        // ── Company (tenant = company) ──
        var company = await identityDb.Companies
            .AsNoTracking()
            .Where(c => c.Id == tenantId)
            .Select(c => new
            {
                c.Name,
                c.Tin,
                c.Address,
                c.OwnerId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (company is not null)
        {
            values["owner_company_name"] = company.Name;
            values["owner_inn"] = company.Tin ?? "";
            values["owner_address"] = company.Address ?? "";

            // ── Director (company owner) ──
            var owner = await identityDb.Users
                .AsNoTracking()
                .Where(u => u.Id == company.OwnerId)
                .Select(u => new
                {
                    FullName = (u.LastName ?? "") + " " + (u.FirstName ?? "") + " " + (u.MiddleName ?? ""),
                    u.PhoneNumber
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (owner is not null)
            {
                values["owner_director"] = owner.FullName.Trim();
                values["owner_phone"] = owner.PhoneNumber ?? "";
            }
        }

        // ── Bank property (main account for tenant) ──
        var bank = await identityDb.BankProperties
            .AsNoTracking()
            .Where(b => b.TenantId == tenantId && b.IsMain)
            .Select(b => new
            {
                b.BankName,
                b.BankMFO,
                b.AccountNumber
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bank is not null)
        {
            values["owner_mfo"] = bank.BankMFO;
            values["owner_bank_account"] = bank.AccountNumber;
            values["owner_bank_name"] = bank.BankName;
        }

        // ── Contract stubs (preview-only) ──
        values["contract_number"] = "б/н (черновик)";
        values["contract_date"] = DateTime.UtcNow.ToString("dd.MM.yyyy");

        return values;
    }
}
