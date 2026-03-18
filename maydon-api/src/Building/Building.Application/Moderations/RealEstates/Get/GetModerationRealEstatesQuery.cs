using System.ComponentModel;
using Building.Domain.Statuses;
using Core.Application.Pagination;

namespace Building.Application.Moderations.RealEstates.Get;

public sealed record GetModerationRealEstatesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	[property: DefaultValue(null)] ModerationStatus? ModerationStatus = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetModerationRealEstatesResponse>(Page, PageSize);
