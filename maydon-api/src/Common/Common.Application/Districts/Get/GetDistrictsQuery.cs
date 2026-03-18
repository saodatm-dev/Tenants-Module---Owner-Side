using System.ComponentModel;
using Core.Application.Pagination;

namespace Common.Application.Districts.Get;

public sealed record GetDistrictsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	[property: DefaultValue(null)] Guid? RegionId = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetDistrictsResponse>(Page, PageSize);
