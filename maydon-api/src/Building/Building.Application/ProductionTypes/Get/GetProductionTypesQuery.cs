using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.ProductionTypes.Get;

public sealed record GetProductionTypesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetProductionTypesResponse>(Page, PageSize);
