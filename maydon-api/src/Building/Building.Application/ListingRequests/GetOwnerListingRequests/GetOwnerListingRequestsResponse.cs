using Building.Domain.ListingRequests;

namespace Building.Application.ListingRequests.GetOwnerListingRequests;

public sealed record GetOwnerListingRequestsResponse(
	Guid Id,
	Guid ClientId,
	string Client,
	string? ClientPhone,
	string? ClientCompany,
	bool IsVerified,
	string? ClientPhoto,
	string Name,
	string Content,
	ListingRequestStatus Status);
