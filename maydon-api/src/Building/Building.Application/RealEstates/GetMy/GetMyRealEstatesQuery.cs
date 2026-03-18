using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.RealEstates.GetMy;

public sealed record GetMyRealEstatesQuery(
	[property: DefaultValue(null)] Guid? RegionId = null,
	[property: DefaultValue(null)] Guid? DistrictId = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetMyRealEstatesResponse>(Page, PageSize);
