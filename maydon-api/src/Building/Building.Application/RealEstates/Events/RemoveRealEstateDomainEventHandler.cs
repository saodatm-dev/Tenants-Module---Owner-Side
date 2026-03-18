using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates.Events;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Events;

internal sealed class RemoveRealEstateDomainEventHandler(
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<RemoveRealEstateDomainEvent>
{
	public async ValueTask Handle(RemoveRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Units

		var existUnits = await dbContext.Units
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.ToListAsync(cancellationToken);

		if (existUnits.Count > 0)
			dbContext.Units.RemoveRange(existUnits.Select(item => item.Remove()));

		#endregion

		#region Rooms

		var existRooms = await dbContext.Rooms
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.ToListAsync(cancellationToken);

		if (existRooms.Count > 0)
			dbContext.Rooms.RemoveRange(existRooms);
		#endregion

		#region Translates

		var existTranslates = await dbContext.RealEstateTranslates
			.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.ToListAsync(cancellationToken);

		if (existTranslates.Count > 0)
			dbContext.RealEstateTranslates.RemoveRange(existTranslates);

		#endregion

		#region Images
		var existImages = await dbContext.RealEstateImages
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.ToListAsync(cancellationToken);

		if (existImages.Count > 0)
		{
			// copying to tenant bucket
			var objectNameTasks = existImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

			await Task.WhenAll(objectNameTasks);

			dbContext.RealEstateImages.RemoveRange(existImages);

		}

		#endregion

		#region Listing
		var existListing = await dbContext.Listings
			.Where(item => item.RealEstateId == @event.RealEstateId)
			.FirstOrDefaultAsync(cancellationToken);

		if (existListing is not null)
			dbContext.Listings.Remove(existListing);

		#endregion

		await ValueTask.CompletedTask;
	}
}
