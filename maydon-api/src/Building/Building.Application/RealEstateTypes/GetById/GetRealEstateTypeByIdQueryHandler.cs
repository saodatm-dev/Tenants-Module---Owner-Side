using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Domain.Languages;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.RealEstateTypes.GetById;

internal sealed class GetRealEstateTypeByIdQueryHandler(IBuildingDbContext dbContext) : IQueryHandler<GetRealEstateTypeByIdQuery, GetRealEstateTypeByIdResponse>
{
	public async Task<Result<GetRealEstateTypeByIdResponse>> Handle(GetRealEstateTypeByIdQuery request, CancellationToken cancellationToken)
	{
		return await (from realEstateType in dbContext.RealEstateTypes
					 .AsNoTracking()
					 .IgnoreQueryFilters([IApplicationDbContext.TranslateFilter])
					 .Where(item => item.Id == request.Id)

					  let names = dbContext.RealEstateTypeTranslates
						 .Where(item =>
							 item.RealEstateTypeId == realEstateType.Id &&
							 item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
						 .Select(item => new LanguageValue(item.LanguageId, item.LanguageShortCode, item.Value))

					  let descriptions = dbContext.RealEstateTypeTranslates
						 .Where(item =>
							 item.RealEstateTypeId == realEstateType.Id &&
							 item.Field == Domain.RealEstateTypes.RealEstateTypeField.Description)
						 .Select(item => new LanguageValue(item.LanguageId, item.LanguageShortCode, item.Value))

					  select new GetRealEstateTypeByIdResponse(
						  realEstateType.Id,
						  realEstateType.TypeName,
						  realEstateType.IconUrl,
						  names,
						  descriptions,
						  realEstateType.ShowBuildingSuggestion,
						  realEstateType.ShowFloorSuggestion,
						  realEstateType.CanHaveUnits,
					  realEstateType.CanHaveMeters,
					  realEstateType.CanHaveFloors))
			 .FirstOrDefaultAsync(cancellationToken);
	}
}
