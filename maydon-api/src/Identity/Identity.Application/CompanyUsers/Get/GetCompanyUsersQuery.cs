using Core.Application.Abstractions.Messaging;
using Core.Application.Pagination;

namespace Identity.Application.CompanyUsers.Get;

public sealed record GetCompanyUsersQuery(string? Filter, int Page, int PageSize) : IQuery<PagedList<GetCompanyUsersResponse>>;
