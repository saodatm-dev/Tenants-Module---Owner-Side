using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.Rooms.Get;

public sealed record GetRoomsQuery(
	Guid RealEstateId,
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRoomsResponse>(Page, PageSize);
