using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Renovations.Get;

public sealed record GetRenovationsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRenovationsResponse>(Page, PageSize);
