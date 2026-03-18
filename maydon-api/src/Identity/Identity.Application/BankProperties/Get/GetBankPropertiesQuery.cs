using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.BankProperties.Get;

public sealed record GetBankPropertiesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetBankPropertiesResponse>(Page, PageSize);
