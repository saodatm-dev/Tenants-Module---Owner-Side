using Core.Application.Pagination;

namespace Building.Application.ListingRequests.GetOwnerListingRequests;

public sealed record GetOwnerListingRequestsQuery(
	int Page = PagedList.DefaultPageValue,
	int PageSize = PagedList.DefaultPageSizeValue) : PagedListQuery<GetOwnerListingRequestsResponse>(Page, PageSize);
