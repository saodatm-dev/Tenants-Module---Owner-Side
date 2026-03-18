using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstateTypes.Events;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Events;

internal sealed class RemoveRealEstateTypeDomainEventHandler(
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<RemoveRealEstateTypeDomainEvent>
{
	public async ValueTask Handle(RemoveRealEstateTypeDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Translates

		var existTranslates = await dbContext.RealEstateTypeTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RealEstateTypeId == @event.RealEstateType.Id)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
			dbContext.RealEstateTypeTranslates.RemoveRange(existTranslates);

		#endregion

		#region Images
		if (!string.IsNullOrWhiteSpace(@event.RealEstateType.IconUrl))
			await fileManager.DeleteFileAsync(@event.RealEstateType.IconUrl, cancellationToken);

		#endregion

		await ValueTask.CompletedTask;
	}
}
