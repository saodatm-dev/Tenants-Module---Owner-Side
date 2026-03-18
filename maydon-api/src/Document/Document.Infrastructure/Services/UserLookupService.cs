using Core.Application.Resources;
using Core.Domain.Results;
using Document.Contract.Gateways;
using Identity.Application.Core.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Document.Infrastructure.Services;

/// <summary>
/// Resolves user information for document owners and signers
/// by querying the Identity users table directly.
/// </summary>
public sealed class UserLookupService(
    IIdentityDbContext identityDbContext,
    ISharedViewLocalizer sharedViewLocalizer) : IUserLookupService
{
    public async Task<Result<UserSnapshot>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await identityDbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserSnapshot(
                u.Id,
                u.FirstName,
                u.LastName,
                u.MiddleName,
                u.Tin,
                u.Pinfl,
                u.Address))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result.Failure<UserSnapshot>(sharedViewLocalizer.ResourceNotFound("User", nameof(userId)))
            : Result.Success(user);
    }

    public async Task<UserSnapshot?> GetByTinOrPinflAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await identityDbContext.Users
            .AsNoTracking()
            .Where(u => u.Tin == identifier || u.Pinfl == identifier)
            .Select(u => new UserSnapshot(
                u.Id,
                u.FirstName,
                u.LastName,
                u.MiddleName,
                u.Tin,
                u.Pinfl,
                u.Address))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await identityDbContext.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<Guid?> GetCompanyIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await identityDbContext.CompanyUsers
            .AsNoTracking()
            .Where(cu => cu.UserId == userId)
            .Select(cu => (Guid?)cu.CompanyId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
