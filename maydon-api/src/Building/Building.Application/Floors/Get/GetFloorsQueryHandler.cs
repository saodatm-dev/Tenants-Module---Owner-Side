using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Floors.Get;

internal sealed class GetFloorsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetFloorsQuery, PagedList<GetFloorsResponse>>
{
	public async Task<Result<PagedList<GetFloorsResponse>>> Handle(GetFloorsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Floors
			.Where(item =>
				(request.BuildingId != null && request.BuildingId != Guid.Empty ? item.BuildingId == request.BuildingId : true) &&
				(request.RealEstateId != null && request.RealEstateId != Guid.Empty ? item.RealEstateId == request.RealEstateId : true) &&
				(!string.IsNullOrWhiteSpace(request.Filter)
					? EF.Functions.Like(item.Number.ToString(), $"%{request.Filter.ToLowerInvariant()}%")
					: true))
			.Include(item => item.RealEstates)
			.Select(item => new GetFloorsResponse(
				item.Id,
				item.Building != null ? item.Building.Number : null,
				dbContext.GetBulkCategoryBuildingTypes(item.RealEstates.Select(item => item.RealEstateTypeId)),
				item.Number,
				item.Type,
				item.Label,
				item.TotalArea,
				item.CeilingHeight,
                item.Plan))
			.AsNoTracking()
			.AsSplitQuery();

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		var tasks = responsesPage.Select(async item =>
		{
			var resolvedPlan = await fileUrlResolver.ResolveUrlAsync(item.Plan, cancellationToken);
			return item with { Plan = resolvedPlan };
		});
		var resolvedPage = (await Task.WhenAll(tasks)).ToList();

		return new PagedList<GetFloorsResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
	}
}
