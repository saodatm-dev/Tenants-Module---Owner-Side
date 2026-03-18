namespace Building.Domain.ListingRequests;

public static class ListingRequestSpec
{
	extension(IQueryable<ListingRequest> query)
	{
		public IQueryable<ListingRequest> Clients(Guid clientId) =>
			query.Where(item => item.ClientId == clientId);

		public IQueryable<ListingRequest> Owners(Guid ownerId) =>
			query.Where(item => item.OwnerId == ownerId);
	}
}
