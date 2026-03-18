namespace Didox.Application.Contracts.DidoxAccounts.Responses;

public record DidoxAccountResponse(
    Guid Id, 
    string Login, 
    string Password,
    DateTime CreatedAt, 
    DateTime? UpdatedAt, 
    Guid? UpdatedBy, 
    DateTime? DeletedAt, 
    Guid? DeletedBy, 
    Guid? OwnerId,
    string? Pinfl,
    string? Tin,
    bool IsDeleted);
