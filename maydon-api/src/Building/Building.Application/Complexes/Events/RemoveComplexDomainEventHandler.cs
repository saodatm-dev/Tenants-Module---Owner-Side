using Building.Application.Core.Abstractions.Data;
using Building.Domain.Complexes.Events;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.Events;

internal sealed class RemoveComplexDomainEventHandler(
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<RemoveComplexDomainEvent>
{
	public async ValueTask Handle(RemoveComplexDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Translates	

		var existTranslates = await dbContext.ComplexTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.ComplexId == @event.Id)
			.ToListAsync(cancellationToken);

		if (existTranslates.Any())
			dbContext.ComplexTranslates.RemoveRange(existTranslates);

		#endregion

		#region Images

		var existImages = await dbContext.ComplexImages
			.Where(item => item.ComplexId == @event.Id)
			.ToListAsync(cancellationToken);

		if (existImages.Any())
		{
			var objectNameTasks = existImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

			await Task.WhenAll(objectNameTasks);

			dbContext.ComplexImages.RemoveRange(existImages);
		}

		#endregion

		await ValueTask.CompletedTask;
	}
}
