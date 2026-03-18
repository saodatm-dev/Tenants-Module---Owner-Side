using Building.Domain.Statuses;

namespace Building.Domain.Units;

public static class UnitSpecs
{
	extension(IQueryable<Unit> query)
	{
		public IQueryable<Unit> InModeration() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.InModeration);

		public IQueryable<Unit> IsActive() =>
			query.Where(item => item.Status == Status.Active && item.ModerationStatus == ModerationStatus.Accept);

		public IQueryable<Unit> IsUpdatable() =>
			query.Where(item => item.Status != Status.Booked && item.Status != Status.Rented);
	}
}
