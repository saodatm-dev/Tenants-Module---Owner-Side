using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.WishlistItems.Get;

public sealed record GetWishlistItemsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetWishlistItemsResponse>(Page, PageSize);
