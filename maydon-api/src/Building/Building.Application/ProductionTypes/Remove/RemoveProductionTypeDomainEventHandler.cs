using Building.Application.Core.Abstractions.Data;
using Building.Domain.ProductionTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.ProductionTypes.Remove;

internal sealed class RemoveProductionTypeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveProductionTypeDomainEvent>
{
	public async ValueTask Handle(RemoveProductionTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.ProductionTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ProductionTypeId == @event.Id)
			.ToListAsync(cancellationToken);

		dbContext.ProductionTypeTranslates.RemoveRange(translates);
	}
}
