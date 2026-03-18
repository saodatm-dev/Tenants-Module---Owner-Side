using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.RealEstates.GetById;

internal sealed class GetModerationRealEstateByIdQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationRealEstateByIdQuery, GetModerationRealEstateByIdResponse>
{
	public async Task<Result<GetModerationRealEstateByIdResponse>> Handle(GetModerationRealEstateByIdQuery request, CancellationToken cancellationToken)
	{
		var result = await (from realEstate in dbContext.RealEstates
			.AsNoTracking()
			.IgnoreQueryFilters([IApplicationDbContext.TenantIdFilter])
			.Where(item => item.Id == request.Id)
			.Include(item => item.Images)

					  let typeName = dbContext.RealEstateTypeTranslates
						  .Where(item =>
							  item.RealEstateTypeId == realEstate.RealEstateTypeId &&
							  item.Field == Domain.RealEstateTypes.RealEstateTypeField.Name)
						  .Select(item => item.Value)
						  .FirstOrDefault()

					  select new GetModerationRealEstateByIdResponse(
							realEstate.Id,
							realEstate.OwnerId,
							realEstate.Status,
							realEstate.ModerationStatus,
							realEstate.Reason,
							typeName,
							realEstate.BuildingNumber,
							realEstate.FloorNumber,
							realEstate.Number,
							realEstate.RoomsCount,
							realEstate.TotalArea,
							realEstate.LivingArea,
							realEstate.CeilingHeight,
							realEstate.CadastralNumber,
							realEstate.RegionId != null ? dbContext.GetRegionName(realEstate.RegionId.Value) : string.Empty,
							realEstate.DistrictId != null ? dbContext.GetDistrictName(realEstate.DistrictId.Value) : string.Empty,
							realEstate.Location != null ? realEstate.Location.X : null,
							realEstate.Location != null ? realEstate.Location.Y : null,
							realEstate.Address,
							realEstate.Plan,
							realEstate.CreatedAt,
							realEstate.Images != null && realEstate.Images.Any() ? realEstate.Images.Select(image => image.ObjectName) : null))
			.FirstOrDefaultAsync(cancellationToken);

		if (result is null)
			return Result<GetModerationRealEstateByIdResponse>.None;

		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(result.ObjectNames, cancellationToken);
		return result with { ObjectNames = resolvedImages };
	}
}

