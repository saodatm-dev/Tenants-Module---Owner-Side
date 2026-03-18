using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Mappings;
using Document.Contract.ContractTemplates.Queries;
using Document.Contract.ContractTemplates.Responses;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.ContractTemplates.Queries.GetList;

public sealed class GetContractTemplatesQueryHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext)
    : IQueryHandler<GetContractTemplatesQuery, PagedList<ContractTemplateListResponse>>
{
    public async Task<Result<PagedList<ContractTemplateListResponse>>> Handle(
        GetContractTemplatesQuery query,
        CancellationToken cancellationToken)
    {
        var queryable = db.ContractTemplates
            .AsNoTracking()
            .Where(t => t.TenantId == executionContext.TenantId);

        if (query.Scope.HasValue)
            queryable = queryable.Where(t => t.Scope == query.Scope.Value.ToDomain());

        if (query.Category.HasValue)
            queryable = queryable.Where(t => t.Category == query.Category.Value.ToDomain());

        if (query.IsActive.HasValue)
            queryable = queryable.Where(t => t.IsActive == query.IsActive.Value);

        var totalCount = await queryable.CountAsync(cancellationToken);

        var skip = (query.PageNumber - 1) * query.PageSize;
        var items = await queryable
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(query.PageSize)
            .Select(t => t.ToListResponse(executionContext.LanguageShortCode))
            .ToListAsync(cancellationToken);

        return Result.Success(new PagedList<ContractTemplateListResponse>(
            items, skip, query.PageSize, totalCount));
    }
}
