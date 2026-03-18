using System.ComponentModel;
using Building.Domain.Statuses;
using Core.Application.Pagination;

namespace Building.Application.Moderations.Get;

public sealed record GetModerationRealEstateUnitsQuery(
	[property: DefaultValue(null)] string? Filter = null,
	[property: DefaultValue(null)] ModerationStatus? ModerationStatus = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetModerationRealEstateUnitsResponse>(Page, PageSize);
