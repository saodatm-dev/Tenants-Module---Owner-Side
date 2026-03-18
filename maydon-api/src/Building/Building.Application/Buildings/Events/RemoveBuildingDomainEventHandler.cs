using Building.Application.Core.Abstractions.Data;
using Building.Domain.Buildings.Events;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Buildings.Events;

internal sealed class RemoveBuildingDomainEventHandler(
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<RemoveBuildingDomainEvent>
{
	public async ValueTask Handle(RemoveBuildingDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Translates	

		var existTranslates = await dbContext.BuildingTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.BuildingId == @event.Id)
			.ToListAsync(cancellationToken);

		if (existTranslates.Any())
			dbContext.BuildingTranslates.RemoveRange(existTranslates);

		#endregion

		#region Images

		var existImages = await dbContext.BuildingImages
			.Where(item => item.BuildingId == @event.Id)
			.ToListAsync(cancellationToken);

		if (existImages.Any())
		{
			var objectNameTasks = existImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

			await Task.WhenAll(objectNameTasks);

			dbContext.BuildingImages.RemoveRange(existImages);
		}

		#endregion

		await ValueTask.CompletedTask;
	}
}
