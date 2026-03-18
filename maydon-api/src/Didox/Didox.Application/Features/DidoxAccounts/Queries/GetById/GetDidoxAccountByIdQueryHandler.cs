using Core.Domain.Results;
using Core.Application.Resources;
using Core.Application.Abstractions.Messaging;
using Didox.Application.Abstractions.Database;
using Didox.Application.Mappings;
using Didox.Application.Contracts.DidoxAccounts.Queries;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Didox.Application.Features.DidoxAccounts.Queries.GetById;

public class GetDidoxAccountByIdQueryHandler(
    IDidoxDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer)
    : IQueryHandler<GetDidoxAccountByIdQuery, DidoxAccountResponse>
{
    public async Task<Result<DidoxAccountResponse>> Handle(GetDidoxAccountByIdQuery query, CancellationToken cancellationToken = default)
    {
        var account = await dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.Id == query.Id && !a.IsDeleted)
            .Select(a => a.ToResponse())
            .FirstOrDefaultAsync(cancellationToken);

        return account is null ? 
            Result.Failure<DidoxAccountResponse>(sharedViewLocalizer.ResourceNotFound("Account", nameof(GetDidoxAccountByIdQuery.Id))) 
            : Result.Success(account);
    }
}


