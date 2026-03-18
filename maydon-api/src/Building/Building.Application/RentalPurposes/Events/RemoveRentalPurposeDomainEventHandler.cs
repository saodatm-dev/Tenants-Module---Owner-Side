using Building.Application.Core.Abstractions.Data;
using Building.Domain.RentalPurposes.Events;
using Core.Application.Abstractions.Data;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RentalPurposes.Events;

internal sealed class RemoveRentalPurposeDomainEventHandler(IBuildingDbContext dbContext) : IDomainEventHandler<RemoveRentalPurposeDomainEvent>
{
	public async ValueTask Handle(RemoveRentalPurposeDomainEvent @event, CancellationToken cancellationToken)
	{
		var rentalPurposeTranslates = await dbContext.RentalPurposeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RentalPurposeId == @event.Id)
			.ToListAsync(cancellationToken);

		if (rentalPurposeTranslates?.Count > 0)
			dbContext.RentalPurposeTranslates.RemoveRange(rentalPurposeTranslates);

		await ValueTask.CompletedTask;
	}
}
