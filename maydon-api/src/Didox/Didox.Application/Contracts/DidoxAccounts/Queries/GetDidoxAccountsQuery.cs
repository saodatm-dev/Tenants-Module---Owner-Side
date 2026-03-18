using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;
using Core.Domain.Results;
using Didox.Application.Contracts.DidoxAccounts.Responses;

namespace Didox.Application.Contracts.DidoxAccounts.Queries;

public record GetDidoxAccountsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? OwnerId = null
) : IQuery<PagedList<DidoxAccountResponse>>;

