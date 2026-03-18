using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Didox.Application.Abstractions.Database;
using Didox.Application.Mappings;
using Didox.Application.Contracts.DidoxAccounts.Queries;
using Didox.Application.Contracts.DidoxAccounts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Didox.Application.Features.DidoxAccounts.Queries.GetList;

public class GetDidoxAccountsQueryHandler(
    IDidoxDbContext dbContext)
    : IQueryHandler<GetDidoxAccountsQuery, PagedList<DidoxAccountResponse>>
{
    public async Task<Result<PagedList<DidoxAccountResponse>>> Handle(GetDidoxAccountsQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = dbContext.Accounts
            .AsNoTracking()
            .Where(a => !a.IsDeleted);

        if (query.OwnerId.HasValue)
        {
            baseQuery = baseQuery.Where(a => a.OwnerId == query.OwnerId.Value);
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return Result.Success(PagedList<DidoxAccountResponse>.Empty(query.PageNumber, query.PageSize));
        }

        var orderedQuery = baseQuery.OrderByDescending(a => a.CreatedDate);

        var skip = (query.PageNumber - 1) * query.PageSize;

        var accounts = await orderedQuery
            .Skip(skip)
            .Take(query.PageSize)
            .Select(a => a.ToResponse())
            .ToListAsync(cancellationToken);

        var result = new PagedList<DidoxAccountResponse>(
            accounts,
            skip,
            query.PageSize,
            totalCount
        );

        return Result.Success(result);
    }
}


