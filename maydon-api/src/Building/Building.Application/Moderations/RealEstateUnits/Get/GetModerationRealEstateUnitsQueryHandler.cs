using Building.Application.Core.Abstractions.Data;
using Building.Domain.Units;
using Core.Application.Abstractions.Messaging;
using Core.Application.Abstractions.Services.Minio;
using Core.Application.Pagination;
using Core.Domain.Results;
using Microsoft.EntityFrameworkCore;

namespace Building.Application.Moderations.Get;

internal sealed class GetModerationRealEstateUnitsQueryHandler(
	IBuildingDbContext dbContext,
	IFileUrlResolver fileUrlResolver) : IQueryHandler<GetModerationRealEstateUnitsQuery, PagedList<GetModerationRealEstateUnitsResponse>>
{
	public async Task<Result<PagedList<GetModerationRealEstateUnitsResponse>>> Handle(GetModerationRealEstateUnitsQuery request, CancellationToken cancellationToken)
	{
		var query = dbContext.Units
			.AsNoTracking()
			.Where(item =>
				!string.IsNullOrWhiteSpace(request.Filter) && !string.IsNullOrEmpty(item.RoomNumber)
				? EF.Functions.Like(item.RoomNumber.ToLower(), $"%{request.Filter.ToLowerInvariant()}%")
				: true)
			.Where(item => !request.ModerationStatus.HasValue || item.ModerationStatus == request.ModerationStatus.Value)
			.Select(item => new GetModerationRealEstateUnitsResponse(
				item.Id,
				item.OwnerId,
				item.RoomNumber,
				item.FloorNumber,
				item.TotalArea,
				item.CeilingHeight,
				item.Status,
				item.ModerationStatus,
				item.CreatedAt,
				item.Images != null && item.Images.Any() ? item.Images : null));

		int totalCount = await query.CountAsync(cancellationToken);

		var responsesPage = await query
			.Skip(request.Page)
			.Take(request.PageSize)
			.ToListAsync(cancellationToken);

		// Resolve image URLs
		var resolvedItems = new List<GetModerationRealEstateUnitsResponse>(responsesPage.Count);
		foreach (var item in responsesPage)
		{
			var resolvedImages = await fileUrlResolver.ResolveUrlsAsync(item.ObjectNames, cancellationToken);
			resolvedItems.Add(item with { ObjectNames = resolvedImages });
		}

		return new PagedList<GetModerationRealEstateUnitsResponse>(resolvedItems, request.Page, request.PageSize, totalCount);
	}
}
