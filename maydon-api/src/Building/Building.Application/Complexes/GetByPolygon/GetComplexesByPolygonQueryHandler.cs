using Building.Application.Core.Abstractions.Data;
using Building.Application.Core.Abstractions.Services;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.GetByPolygon;

internal sealed class GetComplexesByPolygonQueryHandler(
	IBuildingDbContext dbContext,
	IGeometryService geometryService,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetComplexesByPolygonQuery, IEnumerable<GetComplexesByPolygonResponse>>
{
	public async Task<Result<IEnumerable<GetComplexesByPolygonResponse>>> Handle(GetComplexesByPolygonQuery request, CancellationToken cancellationToken)
	{
		var polygon = geometryService.CreatePolygon(
				request.TopLeftLatitude,
				request.TopLeftLongitude,
				request.BottomRightLatitude,
				request.BottomRightLongitude);

		var complexes = await dbContext.Complexes
			.AsNoTracking()
			.Where(item => polygon != null ? polygon.Contains(item.Location) : false)
			.Include(item => item.Descriptions)
			.Include(item => item.Images)
			.Select(item => new GetComplexesByPolygonResponse(
				item.Id,
				dbContext.GetRegionName(item.RegionId),
				dbContext.GetDistrictName(item.DistrictId),
				item.Name,
				item.Descriptions != null && item.Descriptions.Any() ? item.Descriptions.First().Value : string.Empty,
				item.IsCommercial,
				item.IsLiving,
				item.Location != null ? item.Location.X : null,
				item.Location != null ? item.Location.Y : null,
				item.Address,
				item.Images != null ? item.Images.Select(item => item.ObjectName) : null))
			.ToListAsync(cancellationToken);

		var tasks = complexes.Select(async item =>
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.Images, cancellationToken);
			return item with { Images = resolvedImages };
		});
		var resolved = (await Task.WhenAll(tasks)).ToList();

		return resolved;
	}
}

