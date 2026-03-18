using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Categories.Get;

public sealed record GetCategoriesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetCategoriesResponse>(Page, PageSize);
