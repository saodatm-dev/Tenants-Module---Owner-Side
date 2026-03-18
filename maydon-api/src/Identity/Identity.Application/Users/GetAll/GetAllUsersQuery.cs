using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Users.GetAll;

public sealed record GetAllUsersQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetAllUsersResponse>(Page, PageSize);
