using Building.Domain.MeterTariffs;

namespace Building.Domain.RealEstates;

public static class MeterTariffSpecs
{
	extension(IQueryable<MeterTariff> query)
	{
		public IQueryable<MeterTariff> IsActive(DateOnly date) =>
			query.Where(item => item.ValidUntil != null ? item.ValidUntil > date : true);
	}
}
