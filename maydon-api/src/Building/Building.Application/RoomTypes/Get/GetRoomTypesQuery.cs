using System.ComponentModel;
using Core.Application.Pagination;

namespace Building.Application.RoomTypes.Get;

public sealed record GetRoomTypesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetRoomTypesResponse>(Page, PageSize);
