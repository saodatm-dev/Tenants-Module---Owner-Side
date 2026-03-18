using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.RealEstates.Get;

public sealed record GetRealEstatesQuery(
	[property: DefaultValue(null)] Guid? RealEstateTypeId = null,
	[property: DefaultValue(null)] Guid? BuildingId = null,
	[property: DefaultValue(null)] Guid? RegionId = null,
	[property: DefaultValue(null)] Guid? DistrictId = null,
	[property: DefaultValue(null)] Guid? RenovationId = null,
	[property: DefaultValue(null)] int? RoomsCount = null,
	[property: DefaultValue(null)] short? FloorNumber = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRealEstatesResponse>(Page, PageSize);
