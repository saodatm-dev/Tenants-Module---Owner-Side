using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Roles.Get;

public sealed record GetRolesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRolesResponse>(Page, PageSize);
