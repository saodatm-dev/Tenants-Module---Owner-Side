using Building.Domain.Statuses;

namespace Building.Domain.Listings;

public static class ListingSpecs
{
	extension(IQueryable<Listing> query)
	{
		public IQueryable<Listing> InModeration() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.InModeration);

		public IQueryable<Listing> IsActive() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.Accept);

		public IQueryable<Listing> IsUpdatable() =>
			query.Where(item => item.Status != Status.Booked && item.Status != Status.Rented);
	}

	extension(Listing listing)
	{
		public bool IsActive() => listing.Status == Status.Active && listing.ModerationStatus == ModerationStatus.Accept;
	}
}
