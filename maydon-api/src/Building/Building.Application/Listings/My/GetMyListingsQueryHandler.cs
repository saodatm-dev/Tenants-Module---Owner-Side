using Building.Application.Core.Abstractions.Data;
using Core.Application.Abstractions.Authentication;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Listings.My;

internal sealed class GetMyListingsQueryHandler(
    IExecutionContextProvider executionContextProvider,
    IBuildingDbContext dbContext,
    IFileUrlResolver fileUrlResolver) : IQueryHandler<GetMyListingsQuery, PagedList<GetMyListingsResponse>>
{
    public async Task<Result<PagedList<GetMyListingsResponse>>> Handle(GetMyListingsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Listings
            .AsNoTracking()
            .Where(item => item.OwnerId == executionContextProvider.TenantId &&
            (!string.IsNullOrWhiteSpace(request.Filter)
                ? (!string.IsNullOrEmpty(item.BuildingNumber) ? EF.Functions.Like(item.BuildingNumber.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
                  (!string.IsNullOrEmpty(item.Address) ? EF.Functions.Like(item.Address.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false) ||
                  (!string.IsNullOrEmpty(item.ComplexName) ? EF.Functions.Like(item.ComplexName.ToLower(), $"%{request.Filter.ToLowerInvariant()}%") : false)
                : true))
            .Select(item => new GetMyListingsResponse(
                item.Id,
                item.OwnerId,
                item.Title,
                item.ModerationStatus,
                dbContext.GetListingCategoryNamesByIds(item.CategoryIds),
                item.BuildingNumber,
                item.FloorNumbers != null ? item.FloorNumbers.ToList() : null,
                item.RoomsCount,
                item.TotalArea,
                item.LivingArea,
                item.CeilingHeight,
                item.RealEstate.Images != null && item.RealEstate.Images.Any() ? item.RealEstate.Images.Select(image => image.ObjectName) : null,
                item.RegionId != null ? dbContext.GetRegionName(item.RegionId.Value) : string.Empty,
                item.DistrictId != null ? dbContext.GetDistrictName(item.DistrictId.Value) : string.Empty,
                item.Location != null ? item.Location.X : null,
                item.Location != null ? item.Location.Y : null,
                item.Address,
                item.Status,
                item.Reason));

        int totalCount = await query.CountAsync(cancellationToken);

        var responsesPage = await query
            .Skip(request.Page)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var tasks = responsesPage.Select(async item =>
        {
            var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.ObjectNames, cancellationToken);
            return item with { ObjectNames = resolvedImages };
        });
        var resolvedPage = (await Task.WhenAll(tasks)).ToList();

        return new PagedList<GetMyListingsResponse>(resolvedPage, request.Page, request.PageSize, totalCount);
    }
}

