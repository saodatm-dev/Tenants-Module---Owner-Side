using Building.Application.Core.Abstractions.Data;
using Building.Domain.MeterTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.MeterTypes.Events;

internal sealed class RemoveMeterTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveMeterTypeDomainEvent>
{
	public async ValueTask Handle(RemoveMeterTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.MeterTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.MeterTypeId == @event.Id)
			.ToListAsync(cancellationToken);

		if (translates?.Count > 0)
			dbContext.MeterTypeTranslates.RemoveRange(translates);

		await ValueTask.CompletedTask;
	}
}
