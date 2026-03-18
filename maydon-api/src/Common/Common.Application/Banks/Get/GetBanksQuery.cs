using System.ComponentModel;
using Core.Application.Pagination;

namespace Common.Application.Banks.Get;

public sealed record GetBanksQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetBanksResponse>(Page, PageSize);
