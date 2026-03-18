using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstateUnits.GetById;

internal sealed class GetModerationRealEstateUnitByIdQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationRealEstateUnitByIdQuery, GetModerationRealEstateUnitByIdResponse>
{
	public async Task<Result<GetModerationRealEstateUnitByIdResponse>> Handle(GetModerationRealEstateUnitByIdQuery request, CancellationToken cancellationToken)
	{
		var result = await (from realEstateUnit in dbContext.Units
			.AsNoTracking()
			.Where(item => item.Id == request.Id)

					  let typeName = dbContext.RealEstateTypeTranslates
						  .Where(item =>
							  item.RealEstateTypeId == realEstateUnit.RealEstateTypeId &&
							  item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
						  .Select(item => item.Value)
						  .FirstOrDefault()

					  select new GetModerationRealEstateUnitByIdResponse(
							realEstateUnit.Id,
							realEstateUnit.OwnerId,
							realEstateUnit.Status,
							realEstateUnit.ModerationStatus,
							realEstateUnit.Reason,
							typeName,
							realEstateUnit.FloorNumber,
							realEstateUnit.RoomNumber,
							realEstateUnit.TotalArea,
							realEstateUnit.CeilingHeight,
							realEstateUnit.Plan,
							realEstateUnit.CreatedAt,
							realEstateUnit.Images != null && realEstateUnit.Images.Any() ? realEstateUnit.Images : null))
			.FirstOrDefaultAsync(cancellationToken);

		if (result is null)
			return Result<GetModerationRealEstateUnitByIdResponse>.None;

		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(result.Images, cancellationToken);
		return result with { Images = resolvedImages };
	}
}

