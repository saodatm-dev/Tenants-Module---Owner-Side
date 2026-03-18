using System.ComponentModel;
using Core.Application.Pagination;

namespace Common.Application.Regions.Get;

public sealed record GetRegionsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRegionsResponse>(Page, PageSize);
