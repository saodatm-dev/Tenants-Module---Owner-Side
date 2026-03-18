using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Document.Application.Features.ContractTemplates.Rendering;
using Document.Contract.ContractTemplates.Queries;
using Document.Contract.ContractTemplates.Responses;

namespace Document.Application.Features.ContractTemplates.Queries.GetPlaceholders;

public sealed class GetPlaceholderCatalogQueryHandler
    : IQueryHandler<GetPlaceholderCatalogQuery, PlaceholderCatalogResponse>
{
    public Task<Result<PlaceholderCatalogResponse>> Handle(
        GetPlaceholderCatalogQuery query,
        CancellationToken cancellationToken)
    {
        var catalog = PlaceholderRegistry.GetCatalog();
        return Task.FromResult(Result.Success(catalog));
    }
}
