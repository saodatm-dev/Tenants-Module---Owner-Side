using Building.Application.Core.Abstractions.Data;
using Building.Domain.RealEstates;
using Building.Domain.Units;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstates.GetById;

internal sealed class GetRealEstateByIdQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetRealEstateByIdQuery, GetRealEstateByIdResponse>
{
	public async Task<Result<GetRealEstateByIdResponse>> Handle(GetRealEstateByIdQuery request, CancellationToken cancellationToken)
	{
		var realEstate = await dbContext.RealEstates
			.AsNoTrackingWithIdentityResolution()
			.IsActive()
			.Where(item => item.Id == request.Id)
			.Include(item => item.Building)
			.Include(item => item.Floor)
			.Include(item => item.Images)
			.Include(item => item.Rooms)
			.Include(item => item.Units)
			.Include(item => item.RealEstateAmenities)
			.FirstOrDefaultAsync(cancellationToken);

		if (realEstate is null)
			return Result.Failure<GetRealEstateByIdResponse>(sharedViewLocalizer.NotFound(nameof(request.Id)));

		// Get real estate type name
		var realEstateTypeName = await dbContext.RealEstateTypeTranslates
			.AsNoTracking()
			.Where(item => item.RealEstateTypeId == realEstate.RealEstateTypeId &&
				item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
			.Select(item => item.Value)
			.FirstOrDefaultAsync(cancellationToken);

		// Get renovation name
		string? renovationName = null;
		if (realEstate.RenovationId is not null)
		{
			renovationName = await dbContext.RenovationTranslates
				.AsNoTracking()
				.Where(item => item.RenovationId == realEstate.RenovationId)
				.Select(item => item.Value)
				.FirstOrDefaultAsync(cancellationToken);
		}

		// Get land category name
		string? landCategoryName = null;
		if (realEstate.LandCategoryId is not null)
		{
			landCategoryName = await dbContext.LandCategoryTranslates
				.AsNoTracking()
				.Where(item => item.LandCategoryId == realEstate.LandCategoryId)
				.Select(item => item.Value)
				.FirstOrDefaultAsync(cancellationToken);
		}

		// Get production type name
		string? productionTypeName = null;
		if (realEstate.ProductionTypeId is not null)
		{
			productionTypeName = await dbContext.ProductionTypeTranslates
				.AsNoTracking()
				.Where(item => item.ProductionTypeId == realEstate.ProductionTypeId)
				.Select(item => item.Value)
				.FirstOrDefaultAsync(cancellationToken);
		}

		// Get region and district names
		var regionName = realEstate.RegionId is not null ? dbContext.GetRegionName(realEstate.RegionId.Value) : null;
		var districtName = realEstate.DistrictId is not null ? dbContext.GetDistrictName(realEstate.DistrictId.Value) : null;

		// Get images
		var imageKeys = realEstate.Images?.Select(img => img.ObjectName).ToList();
		var images = (await fileUrlResolver.ResolveUrlsAsync(imageKeys, cancellationToken)).ToList();

		// Resolve plan to presigned URL
		var plan = await fileUrlResolver.ResolveUrlAsync(realEstate.Plan, cancellationToken);

		// Map rooms (direct rooms without unit)
		var rooms = await MapRoomsAsync(
			realEstate.Rooms?.ToList(),
			cancellationToken);

		// Map units with their rooms
		var units = await MapUnitsAsync(realEstate.Units?.ToList(), cancellationToken);

		// Map amenities
		var amenities = await MapAmenitiesAsync(realEstate.RealEstateAmenities?.ToList(), cancellationToken);

		return new GetRealEstateByIdResponse(
			realEstate.Id,
			realEstate.RealEstateTypeId,
			realEstateTypeName,
			realEstate.TotalArea,
			realEstate.RenovationId,
			renovationName,
			realEstate.LandCategoryId,
			landCategoryName,
			realEstate.ProductionTypeId,
			productionTypeName,
			realEstate.CadastralNumber,
			realEstate.Number,
			realEstate.BuildingId,
			realEstate.BuildingNumber,
			realEstate.FloorId.HasValue ? new[] { realEstate.FloorId.Value } : null,
			realEstate.FloorNumber,
			realEstate.TotalFloors,
			realEstate.AboveFloors,
			realEstate.BelowFloors,
			realEstate.RegionId,
			regionName,
			realEstate.DistrictId,
			districtName,
			realEstate.Address,
			realEstate.Location?.X,
			realEstate.Location?.Y,
			realEstate.LivingArea,
			realEstate.CeilingHeight,
			realEstate.RoomsCount,
			plan,
			images,
			rooms,
			units,
			amenities,
			realEstate.Status.ToString(),
			realEstate.ModerationStatus.ToString(),
			realEstate.CreatedAt,
			realEstate.UpdatedAt);
	}

	private async Task<List<RealEstateRoomResponse>?> MapRoomsAsync(
		List<Domain.Rooms.Room>? rooms,
		CancellationToken cancellationToken)
	{
		if (rooms is null || !rooms.Any())
			return null;

		var roomTypeIds = rooms.Select(r => r.RoomTypeId).Distinct().ToList();

		var roomTypeNames = await dbContext.RoomTypeTranslates
			.AsNoTracking()
			.Where(item => roomTypeIds.Contains(item.RoomTypeId))
			.GroupBy(item => item.RoomTypeId)
			.Select(g => new { RoomTypeId = g.Key, Name = g.First().Value })
			.ToDictionaryAsync(x => x.RoomTypeId, x => x.Name, cancellationToken);

		return rooms.Select(room => new RealEstateRoomResponse(
			room.Id,
			room.RoomTypeId,
			roomTypeNames.GetValueOrDefault(room.RoomTypeId),
			room.Area)).ToList();
	}

	private async Task<List<RealEstateUnitResponse>?> MapUnitsAsync(
		List<Unit>? units,
		CancellationToken cancellationToken)
	{
		if (units is null || !units.Any())
			return null;

		var renovationIds = units
			.Where(u => u.RenovationId is not null)
			.Select(u => u.RenovationId!.Value)
			.Distinct()
			.ToList();

		var renovationNames = new Dictionary<Guid, string>();
		if (renovationIds.Any())
		{
			renovationNames = await dbContext.RenovationTranslates
				.AsNoTracking()
				.Where(item => renovationIds.Contains(item.RenovationId))
				.GroupBy(item => item.RenovationId)
				.Select(g => new { RenovationId = g.Key, Name = g.First().Value })
				.ToDictionaryAsync(x => x.RenovationId, x => x.Name, cancellationToken);
		}

		var realEstateTypeIds = units
			.Select(u => u.RealEstateTypeId)
			.Distinct()
			.ToList();

		var realEstateTypeNames = await dbContext.RealEstateTypeTranslates
			.AsNoTracking()
			.Where(item => realEstateTypeIds.Contains(item.RealEstateTypeId) &&
				item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
			.GroupBy(item => item.RealEstateTypeId)
			.Select(g => new { RealEstateTypeId = g.Key, Name = g.First().Value })
			.ToDictionaryAsync(x => x.RealEstateTypeId, x => x.Name, cancellationToken);

		var tasks = units.Select(async unit =>
		{
			var unitImages = unit.Images != null
				? (await fileUrlResolver.ResolveUrlsAsync(unit.Images, cancellationToken)).ToList()
				: null;

			return new RealEstateUnitResponse(
				unit.Id,
				unit.RealEstateTypeId,
				unit.RealEstateTypeId.HasValue ? realEstateTypeNames.GetValueOrDefault(unit.RealEstateTypeId.Value) : string.Empty,
				unit.RenovationId,
				unit.RenovationId.HasValue ? renovationNames.GetValueOrDefault(unit.RenovationId.Value) : null,
				unit.TotalArea,
				unit.FloorNumber,
				unit.RealEstate != null ? unit.RealEstate.LivingArea : null,
				unit.CeilingHeight,
				unit.Plan,
				unitImages,
				unit.Status.ToString(),
				unit.ModerationStatus.ToString());
		});

		return (await Task.WhenAll(tasks)).ToList();
	}

	private async Task<List<RealEstateAmenityResponse>?> MapAmenitiesAsync(
		List<Domain.RealEstateAmenities.RealEstateAmenity>? realEstateAmenities,
		CancellationToken cancellationToken)
	{
		if (realEstateAmenities is null || !realEstateAmenities.Any())
			return null;

		var amenityIds = realEstateAmenities.Select(ra => ra.AmenityId).Distinct().ToList();

		var amenities = await dbContext.Amenities
			.AsNoTracking()
			.Where(a => amenityIds.Contains(a.Id))
			.Include(a => a.Translates)
			.Include(a => a.AmenityCategory)
				.ThenInclude(c => c.Translates)
			.ToListAsync(cancellationToken);

		return amenities.Select(a => new RealEstateAmenityResponse(
			a.Id,
			a.Translates.FirstOrDefault()?.Value ?? string.Empty,
			a.IconUrl,
			a.AmenityCategoryId,
			a.AmenityCategory?.Translates.FirstOrDefault()?.Value)).ToList();
	}
}
