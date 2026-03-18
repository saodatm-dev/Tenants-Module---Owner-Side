using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Units.Get;

public sealed record GetRealEstateUnitsQuery(
	[property: DefaultValue(null)] Guid? RealEstateId = null,
	[property: DefaultValue(null)] Guid? RealEstateTypeId = null,
	[property: DefaultValue(null)] Guid? RenovationId = null,
	[property: DefaultValue(null)] short? FloorNumber = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRealEstateUnitsResponse>(Page, PageSize);
