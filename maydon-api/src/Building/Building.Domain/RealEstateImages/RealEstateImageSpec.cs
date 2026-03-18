using Building.Domain.Statuses;

namespace Building.Domain.RealEstateImages;

public static class RealEstateImageSpec
{
	extension(IQueryable<RealEstateImage> query)
	{
		public IQueryable<RealEstateImage> IsUpdatable() =>
			query.Where(item => item.RealEstate != null && item.RealEstate.Status != Status.Booked && item.RealEstate.Status != Status.Rented);
	}
}
