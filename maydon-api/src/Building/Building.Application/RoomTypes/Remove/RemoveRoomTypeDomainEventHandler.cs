using Building.Application.Core.Abstractions.Data;
using Building.Domain.RoomTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.Remove;

internal sealed class RemoveRoomTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveRoomTypeDomainEvent>
{
	public async ValueTask Handle(RemoveRoomTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var regionTranslates = await dbContext.RoomTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RoomTypeId == @event.Id)
			.ToListAsync(cancellationToken);

		if (regionTranslates?.Count > 0)
			dbContext.RoomTypeTranslates.RemoveRange(regionTranslates);

		await ValueTask.CompletedTask;
	}
}
