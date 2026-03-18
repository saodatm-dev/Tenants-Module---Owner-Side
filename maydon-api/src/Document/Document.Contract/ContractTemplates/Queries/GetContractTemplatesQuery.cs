using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Document.Contract.ContractTemplates.Enums;
using Document.Contract.ContractTemplates.Responses;

namespace Document.Contract.ContractTemplates.Queries;

public sealed record GetContractTemplatesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    ContractTemplateScope? Scope = null,
    ContractTemplateCategory? Category = null,
    bool? IsActive = null
) : IQuery<PagedList<ContractTemplateListResponse>>;
