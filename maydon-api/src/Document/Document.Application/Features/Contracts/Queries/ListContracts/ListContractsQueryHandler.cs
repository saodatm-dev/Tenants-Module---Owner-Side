using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.Contracts.Mappings;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Document.Domain.Contracts.Enums;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Queries.ListContracts;

/// <summary>
/// Returns a paginated list of contracts with optional filtering.
/// </summary>
public sealed class ListContractsQueryHandler(
    IDocumentDbContext dbContext,
    IExecutionContextProvider executionContext)
    : IQueryHandler<ListContractsQuery, PagedContractResponse>
{
    public async Task<Result<PagedContractResponse>> Handle(
        ListContractsQuery query,
        CancellationToken cancellationToken)
    {
        var tenantId = executionContext.TenantId;

        var queryable = dbContext.Contracts
            .AsNoTracking()
            .Include(c => c.FinancialItems)
            .Where(c => c.TenantId == tenantId && !c.IsDeleted)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<ContractStatus>(query.Status, true, out var status))
        {
            queryable = queryable.Where(c => c.Status == status);
        }

        if (query.LeaseId.HasValue)
            queryable = queryable.Where(c => c.LeaseId == query.LeaseId.Value);

        if (query.FromDate.HasValue)
        {
            var fromDateTime = query.FromDate.Value.ToDateTime(TimeOnly.MinValue);
            queryable = queryable.Where(c => c.ContractDate >= fromDateTime);
        }

        if (query.ToDate.HasValue)
        {
            var toDateTime = query.ToDate.Value.ToDateTime(TimeOnly.MaxValue);
            queryable = queryable.Where(c => c.ContractDate <= toDateTime);
        }

        var totalCount = await queryable.CountAsync(cancellationToken);

        var items = await queryable
            .OrderByDescending(c => c.ContractDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        var responses = items.Select(c => c.ToResponse()).ToList();

        return Result.Success(new PagedContractResponse(responses, totalCount, query.Page, query.PageSize));
    }
}
