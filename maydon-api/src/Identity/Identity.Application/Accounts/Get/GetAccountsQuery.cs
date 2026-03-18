using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Accounts.Get;

public sealed record GetAccountsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetAccountsResponse>(Page, PageSize);
