using Didox.Application.Contracts.DidoxAccounts.Responses;
using Didox.Domain.Entities;

namespace Didox.Application.Mappings;

/// <summary>
/// Extension methods for mapping domain entities to response DTOs
/// </summary>
public static class DidoxMappingExtensions
{
    /// <summary>
    /// Maps a DidoxAccount entity to DidoxAccountResponse
    /// </summary>
    public static DidoxAccountResponse ToResponse(this DidoxAccount account)
    {
        return new DidoxAccountResponse(
            Id: account.Id,
            Login: account.Login,
            Password: account.Password,
            CreatedAt: account.CreatedDate,
            UpdatedAt: account.UpdatedAt,
            UpdatedBy: account.UpdatedBy,
            DeletedAt: account.DeletedAt,
            DeletedBy: account.DeletedBy,
            OwnerId: account.OwnerId,
            Pinfl: account.Pinfl,
            Tin: account.Tin,
            IsDeleted: account.IsDeleted
        );
    }
}

