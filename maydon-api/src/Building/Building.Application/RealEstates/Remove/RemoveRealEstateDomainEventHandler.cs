using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates.Events;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Remove;

internal sealed class RemoveRealEstateDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveRealEstateDomainEvent>
{
	public async ValueTask Handle(RemoveRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Rooms
		var existRooms = await dbContext.Rooms
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.ToListAsync(cancellationToken);

		if (existRooms?.Count > 0)
			dbContext.Rooms.RemoveRange(existRooms);

		#endregion

		#region Real estate categories

		//var existItems = await dbContext.RealEstateCategories
		//.Where(item => item.RealEstateId == @event.RealEstateId)
		//.ToListAsync(cancellationToken);

		//if (existItems?.Count > 0)
		//	dbContext.RealEstateCategories.RemoveRange(existItems);

		#endregion

		await ValueTask.CompletedTask;
	}
}
