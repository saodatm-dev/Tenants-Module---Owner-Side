using System.ComponentModel;
using Core.Application.Pagination;

namespace Common.Application.Languages.Get;

public sealed record GetLanguagesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetLanguagesResponse>(Page, PageSize);
