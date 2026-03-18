using Building.Domain.ListingRequests;

namespace Building.Application.ListingRequests.Get;

public sealed record GetClientListingRequestsResponse(
	Guid Id,
	Guid OwnerId,
	string Owner,
	string Name,
	string Content,
	ListingRequestStatus Status);
