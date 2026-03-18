using Building.Application.Core.Abstractions.Data;
using Building.Domain.BuildingImages;
using Building.Domain.Buildings;
using Building.Domain.Buildings.Events;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Buildings.Events;

internal sealed class UpsertBuildingDomainEventHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<UpsertBuildingDomainEvent>
{
	public async ValueTask Handle(UpsertBuildingDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Translates	
		if (@event.Descriptions?.Any() == true)
		{
			var existTranslates = await dbContext.BuildingTranslates
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.BuildingId == @event.BuildingId)
				.ToListAsync(cancellationToken);

			if (existTranslates.Any())
			{
				// update
				foreach (var translate in @event.Descriptions)
				{
					var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
					if (existTranslate is not null)
						dbContext.BuildingTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
					else
					{
						await dbContext.BuildingTranslates.AddAsync(
							new BuildingTranslate(
								@event.BuildingId,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value)
							, cancellationToken);
					}
				}
			}
			else
			{
				await dbContext.BuildingTranslates.AddRangeAsync(@event
					.Descriptions
					.Select(item =>
						new BuildingTranslate(
							@event.BuildingId,
							item.LanguageId,
							item.LanguageShortCode,
							item.Value))
					, cancellationToken);
			}
		}
		#endregion

		#region Images

		if (@event.Images is not null && @event.Images.Any())
		{
			var existImages = await dbContext.BuildingImages
				.Where(item => item.BuildingId == @event.BuildingId)
				.ToListAsync(cancellationToken);

			if (existImages.Count > 0)
			{
				// update
				var existImageKeys = existImages.Select(image => image.ObjectName).ToHashSet();
				var newImages = @event.Images.Where(image => !existImageKeys.Contains(image));
				var deleteImages = existImages.Where(image => !@event.Images.Contains(image.ObjectName));

				if (newImages.Any())
				{
					await dbContext.BuildingImages.AddRangeAsync(newImages.Select(result =>
						new BuildingImage(
							@event.BuildingId,
							result))
						, cancellationToken);

					// copying to tenant bucket
					//var objectNameTasks = @event.Images
					//	.Select(imageKey =>
					//		fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", cancellationToken: cancellationToken));

					//var results = await Task.WhenAll(objectNameTasks);

					//await dbContext.BuildingImages.AddRangeAsync(results.Select(result =>
					//	new BuildingImage(
					//		@event.BuildingId,
					//		result.Value))
					//	, cancellationToken);


				}
				if (deleteImages.Any())
				{
					// copying to tenant bucket
					//var objectNameTasks = deleteImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

					//await Task.WhenAll(objectNameTasks);

					//dbContext.BuildingImages.RemoveRange(deleteImages);
				}
			}
			else
			{
				// copying to tenant bucket
				//var objectNameTasks = @event.Images
				//	.Select(imageKey =>
				//		fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", cancellationToken: cancellationToken));

				//var results = await Task.WhenAll(objectNameTasks);

				//await dbContext.BuildingImages.AddRangeAsync(results.Select(result =>
				//	new BuildingImage(
				//		@event.BuildingId,
				//		result.Value))
				//	, cancellationToken);

				await dbContext.BuildingImages.AddRangeAsync(@event.Images.Select(result =>
						new BuildingImage(
							@event.BuildingId,
							result))
						, cancellationToken);
			}
		}

		#endregion

		await ValueTask.CompletedTask;
	}
}
