using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Users.Get;

public sealed record GetUsersQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetUsersResponse>(Page, PageSize);
