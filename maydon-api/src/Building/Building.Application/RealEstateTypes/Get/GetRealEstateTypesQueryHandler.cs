using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.Get;

internal sealed class GetRealEstateTypesQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRealEstateTypesQuery, IEnumerable<GetRealEstateTypesResponse>>
{
	public async Task<Result<IEnumerable<GetRealEstateTypesResponse>>> Handle(GetRealEstateTypesQuery request, CancellationToken cancellationToken)
	{
		return await (from realEstateType in dbContext.RealEstateTypes
					 .AsNoTracking()

					  let names = dbContext.RealEstateTypeTranslates
						.Where(item =>
							item.RealEstateTypeId == realEstateType.Id &&
							item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name &&
							(!string.IsNullOrWhiteSpace(request.Filter)
								? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
								: true))
						.Select(item => item.Value)
						.FirstOrDefault()

					  let descriptions = dbContext.RealEstateTypeTranslates
						.Where(item =>
							item.RealEstateTypeId == realEstateType.Id &&
							item.Field == Domain.RealEstateTypes.RealEstateTypeField.Description &&
							(!string.IsNullOrWhiteSpace(request.Filter)
								? EF.Functions.Like(item.Value.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
								: true))
						.Select(item => item.Value)
						.FirstOrDefault()

					  select new GetRealEstateTypesResponse(
						 realEstateType.Id,
						 realEstateType.TypeName,
						 realEstateType.IconUrl,
						 names,
						 descriptions,
						 realEstateType.ShowBuildingSuggestion,
						 realEstateType.ShowFloorSuggestion,
						 realEstateType.CanHaveUnits,
					 realEstateType.CanHaveMeters,
					 realEstateType.CanHaveFloors
					 ))
			 .ToListAsync(cancellationToken);
	}
}
