using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Listings.Get;

public sealed record GetListingsQuery(
	[property: DefaultValue(null)] Guid? CategoryId = null,
	[property: DefaultValue(null)] Guid? RegionId = null,
	[property: DefaultValue(null)] Guid? DistrictId = null,
	[property: DefaultValue(null)] Guid? RenovationId = null,
	[property: DefaultValue(null)] int? RoomsCount = null,
	[property: DefaultValue(null)] short? FromFloorNumber = null,
	[property: DefaultValue(null)] short? ToFloorNumber = null,
	[property: DefaultValue(null)] short? FromSquare = null,
	[property: DefaultValue(null)] short? ToSquare = null,
	[property: DefaultValue(null)] decimal? FromPrice = null,
	[property: DefaultValue(null)] decimal? ToPrice = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetListingsResponse>(Page, PageSize);
