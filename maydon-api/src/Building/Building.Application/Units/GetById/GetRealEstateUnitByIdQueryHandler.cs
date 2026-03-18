using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Resources;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Units.GetById;

internal sealed class GetRealEstateUnitByIdQueryHandler(
	ISharedViewLocalizer sharedViewLocalizer,
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetRealEstateUnitByIdQuery, GetRealEstateUnitByIdResponse>
{
	public async Task<Result<GetRealEstateUnitByIdResponse>> Handle(GetRealEstateUnitByIdQuery request, CancellationToken cancellationToken)
	{
		var maybeItem = await (from unit in dbContext.Units
							   where unit.Id == request.Id

							   let renovationTranslate = dbContext.RenovationTranslates
								   .Where(item => item.RenovationId == unit.RenovationId)
								   .Select(item => item.Value)
								   .FirstOrDefault()

							   select new GetRealEstateUnitByIdResponse(
								   unit.Id,
								   unit.OwnerId,
								   unit.RealEstateId,
								   unit.RealEstateTypeId,
								   unit.FloorNumber,
								   unit.RoomNumber,
								   unit.TotalArea,
								   unit.CeilingHeight,
								   unit.RenovationId,
								   renovationTranslate != null ? renovationTranslate : string.Empty,
								   unit.Plan,
								   unit.Coordinates,
								   unit.Images,
								   unit.Status,
								   unit.ModerationStatus,
								   unit.Reason))
								   .AsNoTrackingWithIdentityResolution()
								   .FirstOrDefaultAsync(cancellationToken);

		if (maybeItem is null)
			return Result.Failure<GetRealEstateUnitByIdResponse>(sharedViewLocalizer.NotFound(nameof(GetRealEstateUnitByIdQuery.Id)));

		var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(maybeItem.Images, cancellationToken);
		return maybeItem with { Images = resolvedImages };
	}
}
