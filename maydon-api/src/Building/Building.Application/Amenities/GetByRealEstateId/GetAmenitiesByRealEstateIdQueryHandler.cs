using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Amenities.GetByRealEstateId;

internal sealed class GetAmenitiesByRealEstateIdQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetAmenitiesByRealEstateIdQuery, IEnumerable<GetAmenitiesByRealEstateIdResponse>>
{
	public async Task<Result<IEnumerable<GetAmenitiesByRealEstateIdResponse>>> Handle(GetAmenitiesByRealEstateIdQuery request, CancellationToken cancellationToken)
	{
		return await (from realEstateAmenity in dbContext.RealEstateAmenities.Where(item => item.RealEstateId == request.RealEstateId)
					  join amenity in dbContext.Amenities on realEstateAmenity.AmenityId equals amenity.Id
					  join amenityCategory in dbContext.AmenityCategories on amenity.AmenityCategoryId equals amenityCategory.Id
					  join amenityCategoryTranslate in dbContext.AmenityCategoryTranslates on amenityCategory.Id equals amenityCategoryTranslate.AmenityCategoryId
					  group amenityCategoryTranslate by new { amenityCategoryTranslate.AmenityCategoryId, amenityCategoryTranslate.Value } into amenityGroup
					  select new GetAmenitiesByRealEstateIdResponse(
						  amenityGroup.Key.Value,
						  dbContext.GetAmenityByCategoryId(amenityGroup.Key.AmenityCategoryId)))
					.AsNoTracking()
					.ToListAsync(cancellationToken);
	}
}
