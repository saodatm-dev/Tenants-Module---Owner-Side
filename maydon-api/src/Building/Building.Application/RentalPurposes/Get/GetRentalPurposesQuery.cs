using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.RentalPurposes.Get;

public sealed record GetRentalPurposesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRentalPurposesResponse>(Page, PageSize);
