using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Amenities.Get;

public sealed record GetAmenitiesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetAmenitiesResponse>(Page, PageSize);
