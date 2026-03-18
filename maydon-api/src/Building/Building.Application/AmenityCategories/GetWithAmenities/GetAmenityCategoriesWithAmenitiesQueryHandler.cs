using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.AmenityCategories.GetWithAmenities;

internal sealed class GetAmenityCategoriesWithAmenitiesQueryHandler(
	IBuildingDbContext dbContext) : IQueryHandler<GetAmenityCategoriesWithAmenitiesQuery, IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>>
{
	public async Task<Result<IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>>> Handle(GetAmenityCategoriesWithAmenitiesQuery request, CancellationToken cancellationToken)
	{
		var categories = await dbContext.AmenityCategoryTranslates
			.Include(item => item.AmenityCategory)
			.Select(item => new
			{
				CategoryId = item.AmenityCategoryId,
				CategoryName = item.Value
			})
			.AsNoTracking()
			.ToListAsync(cancellationToken);

		var result = categories.Select(c => new GetAmenityCategoriesWithAmenitiesResponse(
			c.CategoryId,
			c.CategoryName,
			dbContext.GetAmenityByCategoryId(c.CategoryId)?
				.Select(a => new AmenityItemResponse(a.Id, a.Name, a.IconUrl))
				?? Enumerable.Empty<AmenityItemResponse>()));

		return Result.Success<IEnumerable<GetAmenityCategoriesWithAmenitiesResponse>>(result);
	}
}
