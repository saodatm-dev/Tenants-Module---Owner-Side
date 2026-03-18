using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Complexes.Get;

public sealed record GetComplexesQuery(
	[property: DefaultValue(null)] Guid? RegionId = null,
	[property: DefaultValue(null)] Guid? DistrictId = null,
	[property: DefaultValue(null)] bool? IsCommercial = null,
	[property: DefaultValue(null)] bool? IsLiving = null,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetComplexesResponse>(Page, PageSize);
