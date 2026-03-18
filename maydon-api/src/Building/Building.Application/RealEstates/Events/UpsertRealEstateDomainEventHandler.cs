using Building.Application.Core.Abstractions.Data;
using Building.Domain.Domain.RealEstates.Events;
using Building.Domain.RealEstateAmenities;
using Building.Domain.RealEstateDelegations;
using Building.Domain.RealEstateImages;
using Building.Domain.RealEstates;
using Building.Domain.Rooms;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.Events;

internal sealed class UpsertRealEstateDomainEventHandler(
	IExecutionContextProvider executionContextProvider,
	IBuildingDbContext dbContext,
	IFileManager fileManager) : IDomainEventHandler<UpsertRealEstateDomainEvent>
{
	public async ValueTask Handle(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		await UpsertUnits(@event, cancellationToken);

		await UpsertRooms(@event, cancellationToken);

		await UpsertDescriptions(@event, cancellationToken);

		await UpsertImages(@event, cancellationToken);

		await UpsertRealEstateAmenities(@event, cancellationToken);

		await UpsertRealEstateDelegation(@event, cancellationToken);

		await ValueTask.CompletedTask;
	}
	private async Task UpsertUnits(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Units is not null && @event.Units.Any())
		{
			var existUnits = await dbContext.Units
				.Where(item => item.RealEstateId == @event.RealEstate.Id)
				.ToListAsync(cancellationToken);

			if (existUnits.Count > 0)
			{
				var newUnits = @event.Units.Where(item => !existUnits
					.Any(r => r.FloorNumber == item.FloorNumber));

				var deleteUnits = existUnits.Where(item => !@event.Units
					.Any(r => r.FloorNumber == item.FloorNumber));

				//if (newUnits.Any())
				//{
				//	await dbContext.Units.AddRangeAsync(
				//		newUnits.Select(unit =>
				//			new Unit(
				//				@event.RealEstate.OwnerId,
				//				@event.RealEstate.Id,
				//				@event.RealEstate.RealEstateTypeId,
				//				unit.FloorNumber,
				//				unit.RenovationId,
				//				unit.TotalArea,
				//				unit.LivingArea,
				//				unit.CeilingHeight,
				//				unit.RoomsCount,
				//				unit.Rooms,
				//				unit.Plan,
				//				unit.Coordinates,
				//				unit.Images))
				//		, cancellationToken);
				//}
				//if (deleteUnits.Any())
				//	dbContext.Units.RemoveRange(deleteUnits);
			}
			//else
			//{
			//	await dbContext.Units.AddRangeAsync(
			//		@event.Units.Select(unit =>
			//				new RealEstateUnit(
			//					@event.RealEstate.OwnerId,
			//					@event.RealEstate.Id,
			//					@event.RealEstate.RealEstateTypeId,
			//					unit.FloorNumber,
			//					unit.RenovationId,
			//					unit.TotalArea,
			//					unit.LivingArea,
			//					unit.CeilingHeight,
			//					unit.RoomsCount,
			//					unit.Rooms,
			//					unit.Plan,
			//					unit.Coordinates,
			//					unit.Images))
			//		, cancellationToken);
			//}
		}
	}
	private async Task UpsertRooms(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.RealEstate.Rooms is not null && @event.RealEstate.Rooms.Any())
		{
			var existRooms = await dbContext.Rooms
				.Where(item => item.RealEstateId == @event.RealEstate.Id)
				.ToListAsync(cancellationToken);

			if (existRooms.Count > 0)
			{
				var newRooms = @event.RealEstate.Rooms.Where(item => !existRooms
					.Any(r => r.RoomTypeId == item.RoomTypeId && r.Area == item.Area));

				var deleteRooms = existRooms.Where(item => !@event.RealEstate.Rooms
					.Any(r => r.RoomTypeId == item.RoomTypeId && r.Area == item.Area));

				if (newRooms.Any())
				{
					await dbContext.Rooms.AddRangeAsync(
						newRooms.Select(room =>
							new Room(
								@event.RealEstate.Id,
								room.RoomTypeId,
								null,
								room.Number,
								room.Area)),
						cancellationToken);
				}
				if (deleteRooms.Any())
				{
					dbContext.Rooms.RemoveRange(deleteRooms);
				}
			}
			else
			{
				await dbContext.Rooms.AddRangeAsync(
					@event.RealEstate.Rooms.Select(room =>
						new Domain.Rooms.Room(
							@event.RealEstate.Id,
							room.RoomTypeId,
							null,
							room.Number,
							room.Area)),
					cancellationToken);
			}
		}
	}
	private async Task UpsertDescriptions(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Descriptions is not null && @event.Descriptions.Any())
		{
			var existTranslates = await dbContext.RealEstateTranslates
				.IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
				.Where(item => item.RealEstateId == @event.RealEstate.Id)
				.ToListAsync(cancellationToken);

			if (existTranslates.Count > 0)
			{
				// update
				foreach (var translate in @event.Descriptions)
				{
					var existTranslate = existTranslates.Find(item => item.LanguageId == translate.LanguageId);
					if (existTranslate is not null)
						dbContext.RealEstateTranslates.Update(
							existTranslate.Update(
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value));
					else
						await dbContext.RealEstateTranslates.AddAsync(
							new RealEstateTranslate(
								@event.RealEstate.Id,
								translate.LanguageId,
								translate.LanguageShortCode,
								translate.Value),
							cancellationToken);
				}
			}
			else
			{
				await dbContext.RealEstateTranslates.AddRangeAsync(
					@event.Descriptions.Select(translate =>
						new RealEstateTranslate(
							@event.RealEstate.Id,
							translate.LanguageId,
							translate.LanguageShortCode,
							translate.Value))
					, cancellationToken);
			}
		}
	}
	private async Task UpsertImages(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.Images is not null && @event.Images.Any())
		{
			var existImages = await dbContext.RealEstateImages
				.Where(item => item.RealEstateId == @event.RealEstate.Id)
				.ToListAsync(cancellationToken);

			if (existImages.Count > 0)
			{
				// update
				var existImageKeys = existImages.Select(image => image.ObjectName);
				var newImages = @event.Images.Where(image => !existImageKeys.Contains(image));
				var deleteImages = existImages.Where(image => !@event.Images.Contains(image.ObjectName));

				if (newImages.Any())
				{
					await dbContext.RealEstateImages.AddRangeAsync(newImages.Select(result =>
						new RealEstateImage(
							@event.RealEstate.Id,
							result))
							, cancellationToken);
					// copying to tenant bucket
					//var objectNameTasks = @event.Images
					//	.Select(imageKey =>
					//		fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken));

					//var results = await Task.WhenAll(objectNameTasks);

					//await dbContext.RealEstateImages.AddRangeAsync(results.Select(result =>
					//	new RealEstateImage(
					//		@event.RealEstate.Id,
					//		result.Value))
					//	, cancellationToken);
				}
				if (deleteImages.Any())
				{
					// copying to tenant bucket
					//var objectNameTasks = deleteImages.Select(image => fileManager.DeleteFileAsync(image.ObjectName, cancellationToken));

					//await Task.WhenAll(objectNameTasks);

					//dbContext.RealEstateImages.RemoveRange(deleteImages);
				}
			}
			else
			{
				await dbContext.RealEstateImages.AddRangeAsync(@event.Images.Select(result =>
				new RealEstateImage(
					@event.RealEstate.Id,
					result))
					, cancellationToken);

				// copying to tenant bucket
				//var objectNameTasks = @event.Images
				//	.Select(imageKey =>
				//		fileManager.CopyToPublicAsync(imageKey, $"{executionContextProvider.TenantId}", true, cancellationToken: cancellationToken));

				//var results = await Task.WhenAll(objectNameTasks);

				//await dbContext.RealEstateImages.AddRangeAsync(results.Select(result =>
				//	new RealEstateImage(
				//		@event.RealEstate.Id,
				//		result.Value))
				//	, cancellationToken);
			}
		}
	}
	private async Task UpsertRealEstateDelegation(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (!await dbContext.RealEstateDelegations
			.AsNoTracking()
			.AnyAsync(item =>
				item.OwnerId == executionContextProvider.TenantId &&
				item.RealEstateId == @event.RealEstate.Id
			, cancellationToken))
		{
			await dbContext.RealEstateDelegations.AddAsync(
				new RealEstateDelegation(
					executionContextProvider.TenantId,
					null,
					@event.RealEstate.Id)
				, cancellationToken);
		}

		await ValueTask.CompletedTask;
	}

	private async Task UpsertRealEstateAmenities(UpsertRealEstateDomainEvent @event, CancellationToken cancellationToken)
	{
		if (@event.AmenityIds?.Any() == true)
		{
			var existRealEstateAmenities = await dbContext.RealEstateAmenities
				.Where(item => item.RealEstateId == @event.RealEstate.Id)
				.ToListAsync(cancellationToken);

			if (existRealEstateAmenities.Count > 0)
			{
				var newAmenityIds = @event.AmenityIds.Where(amenityId =>
					!existRealEstateAmenities.Any(r => r.AmenityId == amenityId));
				var deleteAmenities = existRealEstateAmenities.Where(item =>
					!@event.AmenityIds.Any(amenityId => amenityId == item.AmenityId));

				if (newAmenityIds?.Any() == true)
				{
					await dbContext.RealEstateAmenities.AddRangeAsync(
						newAmenityIds.Select(amenityId =>
							new RealEstateAmenity(@event.RealEstate.Id, amenityId)),
						cancellationToken);
				}
				if (deleteAmenities?.Any() == true)
					dbContext.RealEstateAmenities.RemoveRange(deleteAmenities);
			}
			else
			{
				await dbContext.RealEstateAmenities.AddRangeAsync(
					@event.AmenityIds.Select(amenityId =>
						new RealEstateAmenity(@event.RealEstate.Id, amenityId)),
					cancellationToken);
			}
		}

		await ValueTask.CompletedTask;
	}
}
