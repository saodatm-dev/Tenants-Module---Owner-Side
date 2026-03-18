using System.ComponentModel;
using Core.Application.Pagination;

namespace Common.Application.Currencies.Get;

public sealed record GetCurrenciesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetCurrenciesResponse>(Page, PageSize);
