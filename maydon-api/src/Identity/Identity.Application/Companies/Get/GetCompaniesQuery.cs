using System.ComponentModel;
using Core.Application.Pagination;

namespace Identity.Application.Companies.Get;

public sealed record GetCompaniesQuery(
	[property: DefaultValue(null)] string? Filter = null,
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetCompaniesResponse>(Page, PageSize);
