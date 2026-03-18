using Building.Domain.Statuses;

namespace Building.Domain.RealEstates;

public static class RealEstateSpecs
{
	extension(IQueryable<RealEstate> query)
	{
		public IQueryable<RealEstate> InModeration() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.InModeration);

		public IQueryable<RealEstate> IsActive() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.Accept);

		public IQueryable<RealEstate> IsUpdatable(Guid tenantId) =>
			query.Where(item =>
				item.Status != Status.Booked &&
				item.Status != Status.Rented &&
				(item.OwnerId == tenantId ||
				(item.RealEstateDelegation != null && (item.RealEstateDelegation.OwnerId == tenantId || item.RealEstateDelegation.AgentId == tenantId))));
	}
}
