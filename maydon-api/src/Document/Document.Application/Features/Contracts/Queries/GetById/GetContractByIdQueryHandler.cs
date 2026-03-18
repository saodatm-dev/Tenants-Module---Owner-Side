using Core.Application.Abstractions.Messaging;
using Core.Application.Resources;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.Contracts.Mappings;
using Document.Contract.Contracts.Queries;
using Document.Contract.Contracts.Responses;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.Contracts.Queries.GetById;

public sealed class GetContractByIdQueryHandler(
    IDocumentDbContext dbContext,
    ISharedViewLocalizer sharedViewLocalizer)
    : IQueryHandler<GetContractByIdQuery, ContractResponse>
{
    public async Task<Result<ContractResponse>> Handle(
        GetContractByIdQuery query,
        CancellationToken cancellationToken)
    {
        var contract = await dbContext.Contracts
            .AsNoTracking()
            .Include(c => c.FinancialItems.OrderBy(fi => fi.SortOrder))
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (contract is null)
            return Result.Failure<ContractResponse>(
                sharedViewLocalizer.ContractNotFound(nameof(query.Id)));

        return Result.Success(contract.ToResponse());
    }
}
