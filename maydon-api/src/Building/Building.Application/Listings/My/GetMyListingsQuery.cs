using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Listings.My;

public sealed record GetMyListingsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetMyListingsResponse>(Page, PageSize);
