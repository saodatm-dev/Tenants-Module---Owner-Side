using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Floors.Get;

public sealed record GetFloorsQuery(
	Guid? BuildingId = null,
	Guid? RealEstateId = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetFloorsResponse>(Page, PageSize);
