using System.ComponentModel;
using Building.Application.Listings.Get;
using Core.Application.Pagination;

namespace Building.Application.Wishlists.Get;

public sealed record GetWishlistsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetListingsResponse>(Page, PageSize);
