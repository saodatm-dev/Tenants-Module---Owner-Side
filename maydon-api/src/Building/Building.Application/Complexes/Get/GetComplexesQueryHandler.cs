using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Complexes.Get;

internal sealed class GetComplexesQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetComplexesQuery, PagedList<GetComplexesResponse>>
{
	public async Task<Result<PagedList<GetComplexesResponse>>> Handle(GetComplexesQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Complexes
			.AsNoTracking()
			.Where(item =>
			(request.IsCommercial != null ? item.IsCommercial == request.IsCommercial : true) &&
			(request.IsLiving != null ? item.IsLiving == request.IsLiving : true) &&
			(request.RegionId != null && request.RegionId != Guid.Empty ? item.RegionId == request.RegionId : true) &&
			(request.DistrictId != null && request.DistrictId != Guid.Empty ? item.DistrictId == request.DistrictId : true) &&
			(!string.IsNullOrWhiteSpace(request.Filter)
				? EF.Functions.Like(item.Name.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true))
			.Include(item => item.Descriptions)
			.Include(item => item.Images)
			.Select(item => new GetComplexesResponse(
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
				item.Images != null ? item.Images.Select(item => item.ObjectName) : null));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var tasks = responsesPage.Select(async item =>
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.Images, cancellationToken);
			return item with { Images = resolvedImages };
		});
		var resolvedPage = (await Task.WhenAll(tasks)).ToList();

		return new PagedList<GetComplexesResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
