using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Core.Application.Abstractions.Messaging;

namespace Core.Application.Pagination;

public record PagedListQuery<T>(
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : IQuery<PagedList<T>>
{
	[Required]
	[DefaultValue(PagedList.DefaultPageValue)]
	public int Page { get; } = ((Page < PagedList.DefaultPageValue
		? PagedList.DefaultPageValue
		: Page > PagedList.DefaultMaxPageValue
			? PagedList.DefaultMaxPageValue
			: Page) - 1) * PageSize;

	[Required]
	[DefaultValue(PagedList.DefaultPageSizeValue)]
	public int PageSize { get; } = PageSize <= PagedList.DefaultEmptyValue
		? PagedList.DefaultPageSizeValue
		: PageSize > PagedList.DefaultMaxPageSizeValue
			? PagedList.DefaultMaxPageSizeValue
			: PageSize;
}
