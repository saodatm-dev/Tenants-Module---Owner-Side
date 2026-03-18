using Building.Application.Core.Abstractions.Data;
using Building.Domain.Renovations.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Renovations.Remove;

internal sealed class RemoveRenovationDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveRenovationDomainEvent>
{
	public async ValueTask Handle(RemoveRenovationDomainEvent @event, CancellationToken cancellationToken)
	{
		var renovationTranslates = await dbContext.RenovationTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RenovationId == @event.Id)
			.ToListAsync(cancellationToken);

		if (renovationTranslates?.Count > 0)
			dbContext.RenovationTranslates.RemoveRange(renovationTranslates);

		await ValueTask.CompletedTask;
	}
}
