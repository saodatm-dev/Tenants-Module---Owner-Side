using Core.Application.Pagination;

namespace Building.Application.Leases.Get;

public sealed record GetLeasesQuery(
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetLeasesResponse>(Page, PageSize);
