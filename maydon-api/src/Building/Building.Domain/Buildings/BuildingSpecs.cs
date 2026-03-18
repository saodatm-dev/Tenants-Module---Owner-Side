using Building.Domain.Statuses;

namespace Building.Domain.Buildings;

public static class BuildingSpecs
{
	extension(IQueryable<Building> query)
	{
		public IQueryable<Building> IsActive() =>
			query.Where(item => item.Status == Status.Active);


	}
}
