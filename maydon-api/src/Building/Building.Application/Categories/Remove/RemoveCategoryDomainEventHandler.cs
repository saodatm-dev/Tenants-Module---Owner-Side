using Building.Application.Core.Abstractions.Data;
using Building.Domain.Categories.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Categories.Remove;

internal sealed class RemoveCategoryDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveCategoryDomainEvent>
{
	public async ValueTask Handle(RemoveCategoryDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.RoomTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RoomTypeId == @event.Id)
			.ToListAsync(cancellationToken);

		if (translates?.Count > 0)
			dbContext.RoomTypeTranslates.RemoveRange(translates);

		await ValueTask.CompletedTask;
	}
}
