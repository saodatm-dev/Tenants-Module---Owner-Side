using Core.Application.Pagination;

namespace Building.Application.ListingRequests.Get;

public sealed record GetClientListingRequestsQuery(
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetClientListingRequestsResponse>(Page, PageSize);
