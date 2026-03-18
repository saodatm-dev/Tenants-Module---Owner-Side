using Building.Application.Core.Abstractions.Data;
using Building.Domain.RoomTypes;
using Building.Domain.RoomTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RoomTypes.Create;

internal sealed class CreateOrUpdateRoomTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<CreateOrUpdateRoomTypeDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateRoomTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var existTranslates = await dbContext.RoomTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RoomTypeId == @event.RoomTypeId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
		{
			// update
			foreach (var translate in @event.Translates)
			{
				var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
				if (existTranslate is not null)
					dbContext.RoomTypeTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
				else
					await dbContext.RoomTypeTranslates.AddAsync(new RoomTypeTranslate(@event.RoomTypeId, translate.LanguageId, translate.LanguageShortCode, translate.Value), cancellationToken);
			}
		}
		else
		{
			await dbContext.RoomTypeTranslates.AddRangeAsync(@event
				.Translates
				.Select(item =>
					new RoomTypeTranslate(
						@event.RoomTypeId,
						item.LanguageId,
						item.LanguageShortCode,
						item.Value))
				, cancellationToken);
		}

		await ValueTask.CompletedTask;
	}
}
