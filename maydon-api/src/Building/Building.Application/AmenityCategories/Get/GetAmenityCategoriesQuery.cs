using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.AmenityCategories.Get;

public sealed record GetAmenityCategoriesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetAmenityCategoriesResponse>(Page, PageSize);
