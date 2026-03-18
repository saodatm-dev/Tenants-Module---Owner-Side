using Building.Application.Core.Abstractions.Data;
using Building.Domain.Listings.Events;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.Events;

internal sealed class RemoveListingDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveListingDomainEvent>
{
	public async ValueTask Handle(RemoveListingDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Listing amenities

		var existListingAmenities = await dbContext.ListingAmenities
			.Where(item => item.ListingId == @event.ListingId)
			.ToListAsync(cancellationToken);

		if (existListingAmenities.Count > 0)
			dbContext.ListingAmenities.RemoveRange(existListingAmenities);

		#endregion

		await ValueTask.CompletedTask;
	}
}
