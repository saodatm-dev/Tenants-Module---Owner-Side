using Building.Application.Core.Abstractions.Data;
using Building.Domain.Amenities.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.Remove;

internal sealed class RemoveAmenityDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveAmenityDomainEvent>
{
	public async ValueTask Handle(RemoveAmenityDomainEvent @event, CancellationToken cancellationToken)
	{
		var translates = await dbContext.AmenityTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.AmenityId == @event.Id)
			.ToListAsync(cancellationToken);

		if (translates?.Count > 0)
			dbContext.AmenityTranslates.RemoveRange(translates);

		await ValueTask.CompletedTask;
	}
}
