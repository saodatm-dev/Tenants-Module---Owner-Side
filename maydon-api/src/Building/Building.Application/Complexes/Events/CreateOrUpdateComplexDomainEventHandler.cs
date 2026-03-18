using Building.Application.Core.Abstractions.Data;
using Building.Domain.Complexes;
using Building.Domain.Complexes.Events;
using Building.Domain.ComplexImages;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.Events;

internal sealed class CreateOrUpdateComplexDomainEventHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<CreateOrUpdateComplexDomainEvent>
{
	public async ValueTask Handle(CreateOrUpdateComplexDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Translates	

		if (@event.Descriptions is not null && @event.Descriptions.Any())
		{
			var existTranslates = await dbContext.ComplexTranslates
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.ComplexId == @event.Id)
				.ToListAsync(cancellationToken);

			if (existTranslates.Count > 0)
			{
				// update
				foreach (var translate in @event.Descriptions)
				{
					var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
					if (existTranslate is not null)
						dbContext.ComplexTranslates.Update(existTranslate.Update(translate.LanguageId, translate.LanguageShortCode, translate.Value));
					else
					{
						await dbContext.ComplexTranslates.AddAsync(
							new ComplexTranslate(
								@event.Id,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value)
							, cancellationToken);
					}
				}
			}
			else
			{
				await dbContext.ComplexTranslates.AddRangeAsync(@event
					.Descriptions
					.Select(item =>
						new ComplexTranslate(
							@event.Id,
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
			var existImages = await dbContext.ComplexImages
				.Where(item => item.ComplexId == @event.Id)
				.ToListAsync(cancellationToken);

			if (existImages.Count > 0)
			{
				// update
				var existImageKeys = existImages.Select(image => image.ObjectName).ToHashSet();
				var newImages = @event.Images.Where(image => !existImageKeys.Contains(image));
				var deleteImages = existImages.Where(image => !@event.Images.Contains(image.ObjectName));

				if (newImages.Any())
				{
					// copying to tenant bucket
					var objectNameTasks = @event.Images
						.Select(imageKey =>
							fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken));

					var results = await Task.WhenAll(objectNameTasks);

					await dbContext.ComplexImages.AddRangeAsync(results.Select(result =>
						new ComplexImage(
							@event.Id,
							result.Value))
						, cancellationToken);
				}
				if (deleteImages.Any())
				{
					// copying to tenant bucket
					var objectNameTasks = deleteImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

					await Task.WhenAll(objectNameTasks);

					dbContext.ComplexImages.RemoveRange(deleteImages);
				}
			}
			else
			{
				// copying to tenant bucket
				var objectNameTasks = @event.Images
					.Select(imageKey =>
						fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken));

				var results = await Task.WhenAll(objectNameTasks);

				await dbContext.ComplexImages.AddRangeAsync(results.Where(item => item.IsSuccess).Select(result =>
					new ComplexImage(
						@event.Id,
						result.Value))
					, cancellationToken);
			}
		}

		#endregion

		await ValueTask.CompletedTask;
	}
}
