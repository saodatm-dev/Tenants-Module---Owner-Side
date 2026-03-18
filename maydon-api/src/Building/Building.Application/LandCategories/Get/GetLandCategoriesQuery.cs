using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.LandCategories.Get;

public sealed record GetLandCategoriesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetLandCategoriesResponse>(Page, PageSize);
