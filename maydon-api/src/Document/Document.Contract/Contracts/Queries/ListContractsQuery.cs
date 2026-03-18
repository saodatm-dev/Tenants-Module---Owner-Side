using Core.Application.Abstractions.Messaging;
using Document.Contract.Contracts.Responses;

namespace Document.Contract.Contracts.Queries;

/// <summary>
/// Returns a paginated list of contracts with optional filtering.
/// </summary>
public sealed record ListContractsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Status = null,
    Guid? LeaseId = null,
    Guid? TenantId = null,
    DateOnly? FromDate = null,
    DateOnly? ToDate = null) : IQuery<PagedContractResponse>;
