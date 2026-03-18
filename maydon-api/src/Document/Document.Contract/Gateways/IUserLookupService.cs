using Core.Domain.Results;

namespace Document.Contract.Gateways;

/// <summary>
/// Provides user lookup operations for document owner and signer resolution.
/// Reads directly from the Identity users table.
/// </summary>
public interface IUserLookupService
{
    /// <summary>
    /// Gets a user snapshot by their ID.
    /// </summary>
    Task<Result<UserSnapshot>> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user snapshot by TIN or PINFL identifier.
    /// </summary>
    Task<UserSnapshot?> GetByTinOrPinflAsync(string identifier, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a user with the given ID exists.
    /// </summary>
    Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the company ID for a user via the CompanyUsers table.
    /// Returns null if the user has no associated company.
    /// </summary>
    Task<Guid?> GetCompanyIdByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
