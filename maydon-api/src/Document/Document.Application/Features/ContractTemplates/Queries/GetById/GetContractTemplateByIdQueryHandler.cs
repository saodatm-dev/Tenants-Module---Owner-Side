using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Abstractions.Data;
using Document.Application.Features.ContractTemplates.Mappings;
using Document.Contract.ContractTemplates.Queries;
using Document.Contract.ContractTemplates.Responses;
using Microsoft.EntityFrameworkCore;

namespace Document.Application.Features.ContractTemplates.Queries.GetById;

public sealed class GetContractTemplateByIdQueryHandler(
    IDocumentDbContext db,
    IExecutionContextProvider executionContext)
    : IQueryHandler<GetContractTemplateByIdQuery, ContractTemplateResponse>
{
    public async Task<Result<ContractTemplateResponse>> Handle(
        GetContractTemplateByIdQuery query,
        CancellationToken cancellationToken)
    {
        var template = await db.ContractTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == query.Id &&
                                      t.TenantId == executionContext.TenantId,
                cancellationToken);

        if (template is null)
        {
            return Result.Failure<ContractTemplateResponse>(
                Error.NotFound("ContractTemplate.NotFound",
                    $"Contract template {query.Id} not found."));
        }

        return Result.Success(template.ToResponse());
    }
}
