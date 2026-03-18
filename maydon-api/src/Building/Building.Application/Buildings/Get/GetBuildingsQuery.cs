using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Buildings.Get;

public sealed record GetBuildingsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetBuildingsResponse>(Page, PageSize);
