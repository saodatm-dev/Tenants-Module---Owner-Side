using Building.Application.Core.Abstractions.Data;
using Building.Domain.Rooms;
using Building.Domain.Units.Events;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.Events;

internal sealed class UpsertRealEstateUnitDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<UpsertUnitDomainEvent>
{
	public async ValueTask Handle(UpsertUnitDomainEvent @event, CancellationToken cancellationToken)
	{
		await UpsertRooms(@event, cancellationToken);

		await ValueTask.CompletedTask;
	}
	private async Task UpsertRooms(UpsertUnitDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Rooms is not null && @event.Rooms.Any())
		{
			var existRooms = await dbContext.Rooms
				.Where(item => item.RealEstateId == @event.Unit.Id)
				.ToListAsync(cancellationToken);

			if (existRooms.Count > 0)
			{
				var newRooms = @event.Rooms.Where(item => !existRooms
					.Any(r => r.RoomTypeId == item.RoomTypeId && r.Area == item.Area));

				var deleteRooms = existRooms.Where(item => !@event.Rooms
					.Any(r => r.RoomTypeId == item.RoomTypeId && r.Area == item.Area));

				if (newRooms.Any())
				{
					await dbContext.Rooms.AddRangeAsync(
						newRooms.Select(room =>
						new Room(
							@event.Unit.RealEstateId,
							@event.Unit.Id,
							room.RoomTypeId,
							room.Area)),
						cancellationToken);
				}
				if (deleteRooms.Any())
				{
					dbContext.Rooms.RemoveRange(deleteRooms);
				}
			}
			else
			{
				await dbContext.Rooms.AddRangeAsync(
					@event.Rooms.Select(room =>
						new Room(
							@event.Unit.RealEstateId,
							@event.Unit.Id,
							room.RoomTypeId,
							room.Area)),
					cancellationToken);
			}
		}
	}
}
