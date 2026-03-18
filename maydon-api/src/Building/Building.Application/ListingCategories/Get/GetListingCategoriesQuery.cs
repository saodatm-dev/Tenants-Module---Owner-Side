using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.ListingCategories.Get;

public sealed record GetListingCategoriesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetListingCategoriesResponse>(Page, PageSize);
